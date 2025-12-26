use eframe::egui;
use display::{BimView, rhi::{Vertex, DisplayMode}, scene::{ProjectionType, CameraMode}};
use std::sync::{Arc, Mutex};
use egui_wgpu::CallbackTrait;
use eframe::wgpu;
use rfd::FileDialog;
use std::fs::File;
use std::io::BufReader;
use fbxcel_dom::any::AnyDocument;
use fbxcel_dom::v7400::object::geometry::TypedGeometryHandle;
use fbxcel_dom::v7400::object::{TypedObjectHandle, ObjectId};
use fbxcel_dom::v7400::object::model::TypedModelHandle;
// use fbxcel_dom::v7400::object::texture::TextureHandle;
use std::collections::HashMap;
use glam::{Mat4, Vec3, Quat};

pub struct MainWindow {
    command_input: String,
    sidebar_expanded: bool,
    bim_view: Arc<Mutex<Option<BimView>>>,
    show_bg_color_picker: bool,
    context_menu_pos: Option<egui::Pos2>,
    console_lines: Vec<String>,
    selected_mesh: Option<usize>,
    // Zoom Box State
    is_zooming_box: bool,
    zoom_box_start: Option<egui::Pos2>,
}

impl MainWindow {
    pub fn new(cc: &eframe::CreationContext<'_>) -> Self {
        // Install image loaders for SVG
        egui_extras::install_image_loaders(&cc.egui_ctx);

        let bim_view = if let Some(wgpu_render_state) = &cc.wgpu_render_state {
             let device = &wgpu_render_state.device;
             let queue = &wgpu_render_state.queue;
             let format = wgpu_render_state.target_format;
             let view = BimView::new(device, queue, format);
             Arc::new(Mutex::new(Some(view)))
        } else {
             Arc::new(Mutex::new(None))
        };

        Self {
            command_input: String::new(),
            sidebar_expanded: true,
            bim_view,
            show_bg_color_picker: false,
            context_menu_pos: None,
            console_lines: Vec::new(),
            selected_mesh: None,
            is_zooming_box: false,
            zoom_box_start: None,
        }
    }
}

impl MainWindow {
    fn push_console(&mut self, line: impl Into<String>) {
        const MAX_LINES: usize = 500;
        self.console_lines.push(line.into());
        if self.console_lines.len() > MAX_LINES {
            let excess = self.console_lines.len() - MAX_LINES;
            self.console_lines.drain(0..excess);
        }
    }
}

// Wrapper struct to implement CallbackTrait for the closure logic
struct BimViewCallback {
    view: Arc<Mutex<Option<BimView>>>,
}

impl CallbackTrait for BimViewCallback {
    fn prepare(
        &self,
        device: &wgpu::Device,
        queue: &wgpu::Queue,
        _screen_descriptor: &egui_wgpu::ScreenDescriptor,
        _egui_encoder: &mut wgpu::CommandEncoder,
        _callback_resources: &mut egui_wgpu::CallbackResources,
    ) -> Vec<wgpu::CommandBuffer> {
        let mut lock = self.view.lock().unwrap();
        if let Some(view) = lock.as_mut() {
             return view.prepare(device, queue);
        }
        Vec::new()
    }

    fn paint(
        &self,
        _info: egui::PaintCallbackInfo,
        render_pass: &mut wgpu::RenderPass<'static>,
        _callback_resources: &egui_wgpu::CallbackResources,
    ) {
        let lock = self.view.lock().unwrap();
        if let Some(view) = lock.as_ref() {
             view.paint(render_pass);
        }
    }
}

// Helper to load SVG
fn icon_button(ui: &mut egui::Ui, name: &str, tooltip: &str) -> egui::Response {
    let bytes = match name {
        "select" => include_bytes!("../../../interaction/src/assets/select.svg").as_slice(),
        "point" => include_bytes!("../../../interaction/src/assets/point.svg").as_slice(),
        "polyline" => include_bytes!("../../../interaction/src/assets/polyline.svg").as_slice(),
        "curve" => include_bytes!("../../../interaction/src/assets/curve.svg").as_slice(),
        "circle" => include_bytes!("../../../interaction/src/assets/circle.svg").as_slice(),
        "ellipse" => include_bytes!("../../../interaction/src/assets/ellipse.svg").as_slice(),
        "arc" => include_bytes!("../../../interaction/src/assets/arc.svg").as_slice(),
        "rectangle" => include_bytes!("../../../interaction/src/assets/rectangle.svg").as_slice(),
        "polygon" => include_bytes!("../../../interaction/src/assets/polygon.svg").as_slice(),
        "surface" => include_bytes!("../../../interaction/src/assets/surface.svg").as_slice(),
        "box" => include_bytes!("../../../interaction/src/assets/box.svg").as_slice(),
        "extrude" => include_bytes!("../../../interaction/src/assets/extrude.svg").as_slice(),
        "join" => include_bytes!("../../../interaction/src/assets/join.svg").as_slice(),
        "explode" => include_bytes!("../../../interaction/src/assets/explode.svg").as_slice(),
        "trim" => include_bytes!("../../../interaction/src/assets/trim.svg").as_slice(),
        "split" => include_bytes!("../../../interaction/src/assets/split.svg").as_slice(),
        "group" => include_bytes!("../../../interaction/src/assets/group.svg").as_slice(),
        "ungroup" => include_bytes!("../../../interaction/src/assets/ungroup.svg").as_slice(),
        "text" => include_bytes!("../../../interaction/src/assets/text.svg").as_slice(),
        "dimension" => include_bytes!("../../../interaction/src/assets/dimension.svg").as_slice(),
        _ => &[],
    };

    let image = egui::Image::from_bytes(format!("bytes://{}.svg", name), bytes)
        .fit_to_original_size(1.0)
        .tint(egui::Color32::WHITE);

    ui.add(egui::ImageButton::new(image)).on_hover_text(tooltip)
}


impl eframe::App for MainWindow {
    fn update(&mut self, ctx: &egui::Context, _frame: &mut eframe::Frame) {
        // 1. Menu Bar (Rhino-like)
        egui::TopBottomPanel::top("top_panel").show(ctx, |ui| {
            egui::menu::bar(ui, |ui| {
                ui.menu_button("File", |ui| {
                    if ui.button("New").clicked() {}
                    if ui.button("Open...").clicked() {}
                    if ui.button("Save").clicked() {}
                    if ui.button("Save As...").clicked() {}
                    ui.separator();
                    if ui.button("Import...").clicked() {
                        if let Some(path) = FileDialog::new()
                            .add_filter("FBX", &["fbx"])
                            .pick_file()
                        {
                            if let Ok(file) = File::open(&path) {
                                let reader = BufReader::new(file);
                                if let Ok(AnyDocument::V7400(_, doc)) = AnyDocument::from_seekable_reader(reader) {
                                    if let Some(render_state) = _frame.wgpu_render_state() {
                                        let device = &render_state.device;
                                        let mut lock = self.bim_view.lock().unwrap();
                                        if let Some(view) = lock.as_mut() {
                                            
                                            // 1. Cache Resources
                                            let mut geometry_map = HashMap::new();
                                            let mut material_map = HashMap::new();
                                            let mut model_map = HashMap::new();
                                            let mut texture_map = HashMap::new();
                                            
                                            // Capture base path for texture resolution
                                            let fbx_dir = path.parent().map(|p| p.to_path_buf());
                                            let fbm_dir = if let Some(parent) = &fbx_dir {
                                                if let Some(stem) = path.file_stem() {
                                                    Some(parent.join(format!("{}.fbm", stem.to_string_lossy())))
                                                } else {
                                                    None
                                                }
                                            } else {
                                                None
                                            };

                                            for object in doc.objects() {
                                                match object.get_typed() {
                                                    TypedObjectHandle::Geometry(TypedGeometryHandle::Mesh(mesh)) => {
                                                        geometry_map.insert(object.object_id(), mesh);
                                                    }
                                                    TypedObjectHandle::Material(material) => {
                                                        material_map.insert(object.object_id(), material);
                                                    }
                                                    TypedObjectHandle::Model(model) => {
                                                        model_map.insert(object.object_id(), model);
                                                    }
                                                    TypedObjectHandle::Texture(texture) => {
                                                        texture_map.insert(object.object_id(), texture);
                                                    }
                                                    _ => {}
                                                }
                                            }

                                            // Helper to get global transform
                                            fn get_global_transform(
                                                model_id: ObjectId, 
                                                model_map: &HashMap<ObjectId, TypedModelHandle>,
                                            ) -> Mat4 {
                                                if let Some(model) = model_map.get(&model_id) {
                                                    // Helper to get Vec3 from property, default to (0,0,0) or (1,1,1) for scale
                                                    fn get_vec3_prop(props: &fbxcel_dom::v7400::object::property::PropertiesHandle, name: &str, default: (f64, f64, f64)) -> (f64, f64, f64) {
                                                        if let Some(prop) = props.get_property(name) {
                                                            let values = prop.value_part();
                                                            if values.len() >= 3 {
                                                                 let x = values[0].get_f64().unwrap_or(default.0);
                                                                 let y = values[1].get_f64().unwrap_or(default.1);
                                                                 let z = values[2].get_f64().unwrap_or(default.2);
                                                                 return (x, y, z);
                                                            }
                                                        }
                                                        default
                                                    }

                                                    let (t, r, s) = match model {
                                                        TypedModelHandle::Mesh(m) => {
                                                            if let Some(props) = m.direct_properties() {
                                                                (
                                                                    get_vec3_prop(&props, "Lcl Translation", (0.0, 0.0, 0.0)),
                                                                    get_vec3_prop(&props, "Lcl Rotation", (0.0, 0.0, 0.0)),
                                                                    get_vec3_prop(&props, "Lcl Scaling", (1.0, 1.0, 1.0))
                                                                )
                                                            } else {
                                                                ((0.0, 0.0, 0.0), (0.0, 0.0, 0.0), (1.0, 1.0, 1.0))
                                                            }
                                                        },
                                                        TypedModelHandle::Null(n) => {
                                                             if let Some(props) = n.direct_properties() {
                                                                 (
                                                                    get_vec3_prop(&props, "Lcl Translation", (0.0, 0.0, 0.0)),
                                                                    get_vec3_prop(&props, "Lcl Rotation", (0.0, 0.0, 0.0)),
                                                                    get_vec3_prop(&props, "Lcl Scaling", (1.0, 1.0, 1.0))
                                                                 )
                                                             } else {
                                                                 ((0.0, 0.0, 0.0), (0.0, 0.0, 0.0), (1.0, 1.0, 1.0))
                                                             }
                                                        },
                                                        // Fallback for other types
                                                        _ => ((0.0, 0.0, 0.0), (0.0, 0.0, 0.0), (1.0, 1.0, 1.0))
                                                    };

                                                    let local_transform = Mat4::from_scale_rotation_translation(
                                                        Vec3::new(s.0 as f32, s.1 as f32, s.2 as f32),
                                                        Quat::from_euler(glam::EulerRot::XYZ, (r.0 as f32).to_radians(), (r.1 as f32).to_radians(), (r.2 as f32).to_radians()),
                                                        Vec3::new(t.0 as f32, t.1 as f32, t.2 as f32),
                                                    );

                                                    // Determine parent
                                                    let parent_model = match model {
                                                        TypedModelHandle::Mesh(m) => m.parent_model(),
                                                        TypedModelHandle::Null(n) => n.parent_model(),
                                                        _ => None
                                                    };

                                                    if let Some(parent) = parent_model {
                                                        return get_global_transform(parent.object_id(), model_map) * local_transform;
                                                    } else {
                                                        return local_transform;
                                                    }
                                                }
                                                Mat4::IDENTITY
                                            }

                                            // Deduplicate noisy warnings (missing material slot) across polygons.
                                            let mut missing_slot_warnings: std::collections::HashSet<String> = std::collections::HashSet::new();

                                            // 2. Iterate Models and Build Mesh
                                            for (model_id, model_handle) in &model_map {
                                                // Only process Mesh models for rendering geometry
                                                if let TypedModelHandle::Mesh(model) = model_handle {
                                                    let transform = get_global_transform(*model_id, &model_map);
                                                    let model_name = model
                                                        .node()
                                                        .attributes()
                                                        .get(1)
                                                        .and_then(|a| a.get_string())
                                                        .map(|s| s.split('\0').next().unwrap_or(s).trim().to_string())
                                                        .unwrap_or_else(|| format!("{:?}", model_id));
                                                    
                                                    // ===== Extract Material Properties =====
                                                    // FBX meshes are often "multi-material" (per polygon). We must build a table of
                                                    // all materials connected to this model, then apply them per polygon via LayerElementMaterial.
                                                    #[derive(Clone)]
                                                    struct MatParams {
                                                        color: [f32; 3],
                                                        material: [f32; 4], // metallic, roughness, opacity, emission
                                                        diffuse_texture: Option<String>,
                                                    }

                                                    const DEBUG_FBX_MATERIAL_IMPORT: bool = false;
                                                    const DEBUG_FBX_MESH_MAT_SLOTS: bool = false;

                                                    // Gather all materials connected to this model in order.
                                                    // Rhino (and many FBX exporters) allow materials to live on parent nodes / instances.
                                                    // If we only look at `model.materials()`, some instances end up with too few materials,
                                                    // while LayerElementMaterial still references slot 1,2,... -> manifests as mat=None/opaque panes.
                                                    let mut material_ids: Vec<ObjectId> = Vec::new();
                                                    let mut seen_mats: std::collections::HashSet<ObjectId> = std::collections::HashSet::new();

                                                    // Collect from parent chain first, then current model.
                                                    let mut cur: Option<ObjectId> = Some(*model_id);
                                                    while let Some(mid) = cur {
                                                        if let Some(mh) = model_map.get(&mid) {
                                                            // Add connected materials for this node.
                                                            match mh {
                                                                TypedModelHandle::Mesh(m) => {
                                                                    for mat in m.materials() {
                                                                        let id = mat.object_id();
                                                                        if seen_mats.insert(id) {
                                                                            material_ids.push(id);
                                                                        }
                                                                    }
                                                                    cur = m.parent_model().map(|p| p.object_id());
                                                                }
                                                                TypedModelHandle::Null(n) => {
                                                                    cur = n.parent_model().map(|p| p.object_id());
                                                                }
                                                                _ => {
                                                                    cur = None;
                                                                }
                                                            }
                                                        } else {
                                                            break;
                                                        }
                                                    }

                                                    // Build material table aligned with FBX material indices.
                                                    let mut material_table: Vec<MatParams> = Vec::new();
                                                    if material_ids.is_empty() {
                                                        material_table.push(MatParams {
                                                            color: [0.85, 0.85, 0.85],
                                                            material: [0.0, 0.5, 1.0, 0.0],
                                                            diffuse_texture: None,
                                                        });
                                                    } else {
                                                        for material_id in &material_ids {
                                                            // Defaults
                                                            let mut color = [0.85, 0.85, 0.85];
                                                            let mut metallic = 0.0_f32;
                                                            let mut roughness = 0.5_f32;
                                                            let mut opacity = 1.0_f32;
                                                            let mut emission = 0.0_f32;
                                                            let mut mat_name: Option<String> = None;
                                                            let mut is_glass = false;
                                                            let mut diffuse_texture: Option<String> = None;

                                                            if let Some(mat) = material_map.get(material_id) {
                                                                let props = mat.properties();

                                                                // Helper: read first f64 scalar property by trying multiple possible FBX names
                                                                let get_f64_prop_any = |names: &[&str]| -> Option<f64> {
                                                                    for &name in names {
                                                                        if let Some(prop) = props.get_property(name) {
                                                                            if let Some(v) = prop.value_part().get(0).and_then(|v| v.get_f64()) {
                                                                                return Some(v);
                                                                            }
                                                                        }
                                                                    }
                                                                    None
                                                                };

                                                                // Material name
                                                                if let Some(name_node) = mat.node().attributes().get(1) {
                                                                    if let Some(name) = name_node.get_string() {
                                                                        // FBX names sometimes carry suffixes / embedded nulls like "\0\u{1}Material"
                                                                        // which makes debugging and string matching harder.
                                                                        let cleaned = name.split('\0').next().unwrap_or(name).trim().to_string();
                                                                        mat_name = Some(cleaned);
                                                                    }
                                                                }

                                                                // 1) Base Color (Diffuse)
                                                                if let Ok(Some(diffuse)) = props.diffuse_color() {
                                                                    color = [diffuse.r as f32, diffuse.g as f32, diffuse.b as f32];
                                                                } else if let Some(prop) = props.get_property("Diffuse") {
                                                                    let values = prop.value_part();
                                                                    if values.len() >= 3 {
                                                                        if let (Some(r), Some(g), Some(b)) =
                                                                            (values[0].get_f64(), values[1].get_f64(), values[2].get_f64())
                                                                        {
                                                                            color = [r as f32, g as f32, b as f32];
                                                                        }
                                                                    }
                                                                }

                                                                // 2) Apply Diffuse Factor
                                                                if let Some(factor_prop) = props.get_property("DiffuseFactor") {
                                                                    if let Some(factor) = factor_prop.value_part().get(0).and_then(|v| v.get_f64()) {
                                                                        let f = factor as f32;
                                                                        color[0] *= f;
                                                                        color[1] *= f;
                                                                        color[2] *= f;
                                                                    }
                                                                }

                                                                // 3) Opacity/Transparency
                                                                if let Some(trans) = get_f64_prop_any(&[
                                                                    "TransparencyFactor",
                                                                    "Transparency Factor",
                                                                    "Transparency",
                                                                    "TransparentFactor",
                                                                    "Transparent Factor",
                                                                ]) {
                                                                    opacity = (1.0 - trans) as f32;
                                                                }
                                                                if let Some(opac) = get_f64_prop_any(&["Opacity", "Opacity Factor", "OpacityFactor"]) {
                                                                    opacity = opac as f32;
                                                                }

                                                                // 4) Shininess -> Roughness
                                                                if let Some(shine) =
                                                                    get_f64_prop_any(&["Shininess", "ShininessExponent", "Glossiness"])
                                                                {
                                                                    roughness = 1.0 - (shine.min(100.0) / 100.0) as f32;
                                                                    roughness = roughness.max(0.04);
                                                                }

                                                                // 5) Specular Factor -> metallic heuristic
                                                                if let Some(spec) = get_f64_prop_any(&["SpecularFactor", "Specular Factor", "Specular"]) {
                                                                    if spec > 0.8 {
                                                                        metallic = ((spec - 0.8) * 5.0).min(1.0) as f32;
                                                                    }
                                                                }

                                                                // 6) Reflectivity
                                                                if let Some(refl) =
                                                                    get_f64_prop_any(&["ReflectionFactor", "Reflection Factor", "Reflectivity"])
                                                                {
                                                                    metallic = metallic.max(refl as f32);
                                                                }

                                                                // 7) Emission
                                                                if let Some(emit) =
                                                                    get_f64_prop_any(&["EmissiveFactor", "Emissive Factor", "Emission", "EmissionFactor"])
                                                                {
                                                                    emission = emit as f32;
                                                                }

                                                                // 8) Material name heuristics
                                                                if let Some(name) = &mat_name {
                                                                    let name_lower = name.to_lowercase();
                                                                    if name_lower.contains("glass")
                                                                        || name_lower.contains("transparent")
                                                                        || name_lower.contains("玻璃")
                                                                        || name_lower.contains("透明")
                                                                    {
                                                                        is_glass = true;
                                                                        opacity = 0.3;
                                                                        roughness = 0.05;
                                                                        if color[0] > 0.9 && color[1] > 0.9 && color[2] > 0.9 {
                                                                            color = [0.9, 0.95, 1.0];
                                                                        }
                                                                    } else if name_lower.contains("metal")
                                                                        || name_lower.contains("steel")
                                                                        || name_lower.contains("iron")
                                                                        || name_lower.contains("chrome")
                                                                    {
                                                                        metallic = 1.0;
                                                                        roughness = 0.2;
                                                                    } else if name_lower.contains("wood") {
                                                                        roughness = 0.7;
                                                                        metallic = 0.0;
                                                                    } else if name_lower.contains("plastic") {
                                                                        roughness = 0.4;
                                                                        metallic = 0.0;
                                                                    }
                                                                }

                                                                // 9) Brighten very dark materials for visibility (only for opaque-ish)
                                                                let luminance = color[0] * 0.299 + color[1] * 0.587 + color[2] * 0.114;
                                                                if luminance < 0.2 && opacity > 0.9 {
                                                                    let boost = (0.25 - luminance) * 0.5;
                                                                    color[0] = (color[0] + boost).min(1.0);
                                                                    color[1] = (color[1] + boost).min(1.0);
                                                                    color[2] = (color[2] + boost).min(1.0);
                                                                }
                                                                
                                                                // 10) Texture
                                                                // Try standard DiffuseColor connection
                                                                let mut texture_node = mat.diffuse_texture();
                                                                
                                                                // Fallback: Check for "Diffuse" connection or other variations
                                                                if texture_node.is_none() {
                                                                    for source in mat.source_objects() {
                                                                        if let Some(label) = source.label() {
                                                                            if label.eq_ignore_ascii_case("diffuse") || label.eq_ignore_ascii_case("diffusecolor") {
                                                                                if let Some(obj) = source.object_handle() {
                                                                                    if let TypedObjectHandle::Texture(tex) = obj.get_typed() {
                                                                                        texture_node = Some(tex);
                                                                                        break;
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }

                                                                if let Some(tex) = texture_node {
                                                                    let mut raw_filename: Option<String> = None;
                                                                    if let Some(prop) = tex.properties().get_property("FileName") {
                                                                        if let Some(v) = prop.value_part().get(0).and_then(|v| v.get_string()) {
                                                                            raw_filename = Some(v.to_string());
                                                                        }
                                                                    }
                                                                    if raw_filename.is_none() {
                                                                         if let Some(prop) = tex.properties().get_property("RelativeFilename") {
                                                                            if let Some(v) = prop.value_part().get(0).and_then(|v| v.get_string()) {
                                                                                raw_filename = Some(v.to_string());
                                                                            }
                                                                        }
                                                                    }
                                                                    
                                                                    if let Some(fname) = raw_filename {
                                                                        // Try to resolve path
                                                                        let path_buf = std::path::PathBuf::from(&fname);
                                                                        
                                                                        // Helper to check existence
                                                                        let check_path = |p: std::path::PathBuf| -> Option<String> {
                                                                            if p.exists() && p.is_file() {
                                                                                 Some(p.to_string_lossy().to_string())
                                                                            } else {
                                                                                None
                                                                            }
                                                                        };

                                                                        // 1. Check absolute or relative to CWD
                                                                        if let Some(p) = check_path(path_buf.clone()) {
                                                                            diffuse_texture = Some(p);
                                                                        } else if let Some(base) = &fbx_dir {
                                                                            // 2. Try relative to FBX file (handle windows backslashes)
                                                                            let relative_path = fname.replace('\\', "/");
                                                                            let p = base.join(&relative_path);
                                                                            if let Some(res) = check_path(p) {
                                                                                diffuse_texture = Some(res);
                                                                            } else if let Some(file_name) = path_buf.file_name() {
                                                                                // 3. Try flat in FBX dir
                                                                                if let Some(res) = check_path(base.join(file_name)) {
                                                                                    diffuse_texture = Some(res);
                                                                                } 
                                                                                // 4. Try in .fbm folder
                                                                                else if let Some(fbm) = &fbm_dir {
                                                                                    if let Some(res) = check_path(fbm.join(file_name)) {
                                                                                        diffuse_texture = Some(res);
                                                                                    }
                                                                                }
                                                                                
                                                                                // 5. Try in common texture subdirs
                                                                                if diffuse_texture.is_none() {
                                                                                    for subdir in &["textures", "Textures", "images", "Images", "tex", "Tex"] {
                                                                                        if let Some(res) = check_path(base.join(subdir).join(file_name)) {
                                                                                            diffuse_texture = Some(res);
                                                                                            break;
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                        
                                                                        if DEBUG_FBX_MATERIAL_IMPORT {
                                                                            println!("Material {:?} texture resolution: raw={:?} resolved={:?}", mat_name, fname, diffuse_texture);
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                                // Clamp derived values to sane ranges
                                                            opacity = opacity.clamp(0.0, 1.0);
                                                            metallic = metallic.clamp(0.0, 1.0);
                                                            roughness = roughness.clamp(0.04, 1.0);

                                                            // Exporters sometimes leak slight transparency into opaque materials (e.g. 0.99),
                                                            // which makes large surfaces unexpectedly render as translucent.
                                                            // Keep glass behavior intact but snap near-opaque non-glass materials back to opaque.
                                                            if !is_glass && opacity > 0.98 {
                                                                opacity = 1.0;
                                                            }
                                                            // if let Some(name) = &mat_name {
                                                            //     let n = name.to_lowercase();
                                                            //     if (n.contains("混凝土") || n.contains("砼") || n.contains("concrete")) && opacity > 0.5 {
                                                            //         opacity = 1.0;
                                                            //     }
                                                            // }

                                                            if DEBUG_FBX_MATERIAL_IMPORT {
                                                                println!(
                                                                    "FBX material[{}]: name={:?} color={:?} metallic={:.3} roughness={:.3} opacity={:.3} emission={:.3}",
                                                                    material_table.len(),
                                                                    mat_name,
                                                                    color,
                                                                    metallic,
                                                                    roughness,
                                                                    opacity,
                                                                    emission
                                                                );
                                                            }

                                                            material_table.push(MatParams {
                                                                color,
                                                                material: [metallic, roughness, opacity, emission],
                                                                diffuse_texture,
                                                            });
                                                        }
                                                    }
    
                                                    for geo_handle in model.geometry() {
                                                        if let Some(mesh) = geometry_map.get(&geo_handle.object_id()) {
                                                            let geo_id = geo_handle.object_id();
                                                            // 1. Get Control Points
                                                            let mut temp_positions = Vec::new();
                                                            
                                                            if let Some(vertices_node) = mesh.node().children().find(|n| n.name() == "Vertices") {
                                                                if let Some(attr) = vertices_node.attributes().first() {
                                                                    if let Some(raw_coords) = attr.get_arr_f64() {
                                                                        for p in raw_coords.chunks(3) {
                                                                            if p.len() == 3 {
                                                                                let local_pos = Vec3::new(p[0] as f32, p[1] as f32, p[2] as f32);
                                                                                let world_pos = transform.transform_point3(local_pos);
                                                                                temp_positions.push(world_pos);
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
    
                                                            // 2. Get Indices & Build Flat Shaded Mesh (Split Vertices)
                                                            if let Some(indices_node) = mesh.node().children().find(|n| n.name() == "PolygonVertexIndex") {
                                                                if let Some(attr) = indices_node.attributes().first() {
                                                                    if let Some(raw_indices) = attr.get_arr_i32() {
                                                                        // Per-polygon/per-vertex material indices (LayerElementMaterial).
                                                                        // FBX can use MappingInformationType:
                                                                        // - AllSame: single entry
                                                                        // - ByPolygon: one entry per polygon
                                                                        // - ByPolygonVertex: one entry per polygon-vertex (same length as PolygonVertexIndex without the negative terminators)
                                                                        // And ReferenceInformationType:
                                                                        // - Direct: the array value is the material slot
                                                                        // - IndexToDirect: need an index array into the direct array
                                                                        #[derive(Default)]
                                                                        struct LayerMaterial {
                                                                            mapping: Option<String>,
                                                                            reference: Option<String>,
                                                                            direct: Vec<i32>,
                                                                            index: Option<Vec<i32>>,
                                                                        }

                                                                        let mut layer_mat: LayerMaterial = LayerMaterial::default();
                                                                        if let Some(layer_node) = mesh.node().children().find(|n| n.name().starts_with("LayerElementMaterial")) {
                                                                            // Helper closures avoid depending on the concrete NodeHandle type.
                                                                            let read_string_from_child = |name: &str| -> Option<String> {
                                                                                layer_node
                                                                                    .children()
                                                                                    .find(|c| c.name() == name)
                                                                                    .and_then(|c| c.attributes().first())
                                                                                    .and_then(|a| a.get_string())
                                                                                    .map(|s| s.to_string())
                                                                            };

                                                                            let read_i32_array_from_child = |names: &[&str]| -> Option<Vec<i32>> {
                                                                                for &n in names {
                                                                                    if let Some(child) = layer_node.children().find(|c| c.name() == n) {
                                                                                        if let Some(a) = child.attributes().first() {
                                                                                            if let Some(arr) = a.get_arr_i32() {
                                                                                                return Some(arr.to_vec());
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                None
                                                                            };

                                                                            layer_mat.mapping = read_string_from_child("MappingInformationType");
                                                                            layer_mat.reference = read_string_from_child("ReferenceInformationType");

                                                                            // Common: direct array is "Materials"
                                                                            layer_mat.direct = read_i32_array_from_child(&["Materials", "Material"]).unwrap_or_default();

                                                                            // Some exporters use an explicit index array node name.
                                                                            layer_mat.index = read_i32_array_from_child(&[
                                                                                "MaterialsIndex",
                                                                                "MaterialIndex",
                                                                                "MaterialsIndices",
                                                                                "MaterialIndices",
                                                                            ]);

                                                                            // Fallback: if we didn't find a direct array by name, scan all children for an i32 array.
                                                                            if layer_mat.direct.is_empty() {
                                                                                for c in layer_node.children() {
                                                                                    if let Some(a) = c.attributes().first() {
                                                                                        if let Some(arr) = a.get_arr_i32() {
                                                                                            // Heuristic: the "Materials" child is usually the largest i32 array here.
                                                                                            let v = arr.to_vec();
                                                                                            if v.len() > layer_mat.direct.len() {
                                                                                                layer_mat.direct = v;
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }

                                                                        // ===== LayerElementUV =====
                                                                        #[derive(Default)]
                                                                        struct LayerUV {
                                                                            mapping: Option<String>,
                                                                            reference: Option<String>,
                                                                            direct: Vec<f64>,
                                                                            index: Option<Vec<i32>>,
                                                                        }
                                                                        
                                                                        let mut layer_uv = LayerUV::default();
                                                                        if let Some(layer_node) = mesh.node().children().find(|n| n.name().starts_with("LayerElementUV")) {
                                                                             let read_string_from_child = |name: &str| -> Option<String> {
                                                                                layer_node.children().find(|c| c.name() == name)
                                                                                    .and_then(|c| c.attributes().first())
                                                                                    .and_then(|a| a.get_string())
                                                                                    .map(|s| s.to_string())
                                                                            };
                                                                            
                                                                            let read_i32_array_from_child = |names: &[&str]| -> Option<Vec<i32>> {
                                                                                for &n in names {
                                                                                    if let Some(child) = layer_node.children().find(|c| c.name() == n) {
                                                                                        if let Some(a) = child.attributes().first() {
                                                                                            if let Some(arr) = a.get_arr_i32() {
                                                                                                return Some(arr.to_vec());
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                None
                                                                            };

                                                                            let read_f64_array_from_child = |names: &[&str]| -> Option<Vec<f64>> {
                                                                                for &n in names {
                                                                                    if let Some(child) = layer_node.children().find(|c| c.name() == n) {
                                                                                        if let Some(a) = child.attributes().first() {
                                                                                            if let Some(arr) = a.get_arr_f64() {
                                                                                                return Some(arr.to_vec());
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                                None
                                                                            };

                                                                            layer_uv.mapping = read_string_from_child("MappingInformationType");
                                                                            layer_uv.reference = read_string_from_child("ReferenceInformationType");
                                                                            layer_uv.direct = read_f64_array_from_child(&["UV", "UVs"]).unwrap_or_default();
                                                                            layer_uv.index = read_i32_array_from_child(&["UVIndex", "UVIndices"]);
                                                                        }

                                                                        // Split triangles by material slot so translucent parts don't force the whole mesh translucent.
                                                                        let mut buckets: HashMap<i32, (Vec<Vertex>, Vec<u32>)> = HashMap::new();
                                                                        let mut poly_start = 0;
                                                                        let mut poly_index: usize = 0;
                                                                        let mut poly_vert_cursor: usize = 0;

                                                                        // Pre-calculate defaults for fallback logic
                                                                        let default_params = material_table
                                                                            .first()
                                                                            .cloned()
                                                                            .unwrap_or(MatParams {
                                                                                color: [0.85, 0.85, 0.85],
                                                                                material: [0.0, 0.5, 1.0, 0.0],
                                                                                diffuse_texture: None,
                                                                            });
                                                                        let glass_fallback = MatParams {
                                                                            color: [0.0, 0.50, 0.75],
                                                                            material: [0.0, 0.05, 0.30, 0.0],
                                                                            diffuse_texture: None,
                                                                        };
                                                                        
                                                                        let get_uv = |poly_idx: usize, vert_cursor: usize, vertex_idx: i32| -> [f32; 2] {
                                                                            if layer_uv.direct.is_empty() { return [0.0, 0.0]; }
                                                                            
                                                                            let mapping = layer_uv.mapping.as_deref().unwrap_or("ByPolygonVertex");
                                                                            let reference = layer_uv.reference.as_deref().unwrap_or("IndexToDirect");
                                                                            
                                                                            let element_index = match mapping {
                                                                                "ByControlPoint" => if vertex_idx >= 0 { vertex_idx as usize } else { (!vertex_idx) as usize },
                                                                                "ByPolygonVertex" => vert_cursor,
                                                                                "ByPolygon" => poly_idx,
                                                                                "AllSame" => 0,
                                                                                _ => vert_cursor,
                                                                            };
                                                                            
                                                                            let mut uv_index = element_index;
                                                                            if reference == "IndexToDirect" {
                                                                                if let Some(indices) = &layer_uv.index {
                                                                                    if element_index < indices.len() {
                                                                                        uv_index = indices[element_index] as usize;
                                                                                    }
                                                                                }
                                                                            }
                                                                            
                                                                            let base = uv_index * 2;
                                                                            if base + 1 < layer_uv.direct.len() {
                                                                                [layer_uv.direct[base] as f32, 1.0 - layer_uv.direct[base+1] as f32]
                                                                            } else {
                                                                                [0.0, 0.0]
                                                                            }
                                                                        };

                                                                        for (i, &idx) in raw_indices.iter().enumerate() {
                                                                            if idx < 0 {
                                                                                let count = i - poly_start + 1;
                                                                                // Determine material slot for this polygon
                                                                                let mut mat_slot: i32 = 0;
                                                                                let mapping = layer_mat.mapping.as_deref().unwrap_or("");
                                                                                let reference = layer_mat.reference.as_deref().unwrap_or("Direct");
                                                                                let element_index: usize = match mapping {
                                                                                    "AllSame" => 0,
                                                                                    "ByPolygon" => poly_index,
                                                                                    "ByPolygonVertex" => poly_vert_cursor,
                                                                                    _ => poly_index, // fallback
                                                                                };

                                                                                // Resolve material slot from LayerElementMaterial if present
                                                                                if !layer_mat.direct.is_empty() {
                                                                                    let mut slot_from_layer: Option<i32> = None;
                                                                                    if reference == "IndexToDirect" {
                                                                                        if let Some(ref index_arr) = layer_mat.index {
                                                                                            if element_index < index_arr.len() {
                                                                                                let di = index_arr[element_index] as usize;
                                                                                                // Only apply second indirection when `direct` looks like a small lookup table
                                                                                                // (typically <= number of connected materials). Otherwise, many exporters store
                                                                                                // the per-element indices directly in the "Materials" array.
                                                                                                if layer_mat.direct.len() <= material_table.len() {
                                                                                                    if di < layer_mat.direct.len() {
                                                                                                        slot_from_layer = Some(layer_mat.direct[di]);
                                                                                                    }
                                                                                                } else if element_index < layer_mat.direct.len() {
                                                                                                    slot_from_layer = Some(layer_mat.direct[element_index]);
                                                                                                }
                                                                                            }
                                                                                        } else if element_index < layer_mat.direct.len() {
                                                                                            // Some exporters still store the "index" in the direct array
                                                                                            slot_from_layer = Some(layer_mat.direct[element_index]);
                                                                                        }
                                                                                    } else {
                                                                                        if element_index < layer_mat.direct.len() {
                                                                                            slot_from_layer = Some(layer_mat.direct[element_index]);
                                                                                        }
                                                                                    }

                                                                                    if let Some(s) = slot_from_layer {
                                                                                        mat_slot = s;
                                                                                    }
                                                                                }
                                                                                if mat_slot < 0 {
                                                                                    mat_slot = 0;
                                                                                }
                                                                                let mat_slot_usize = mat_slot as usize;
                                                                                // Fallback when FBX references a material slot that isn't actually connected
                                                                                // to this model instance (common with instanced geometry / overrides).
                                                                                // If slot is out of range, we log and (optionally) force a glass-like material
                                                                                // so panes don't render opaque just because the material connection is missing.
                                                                                
                                                                                let mat_params = if let Some(p) = material_table.get(mat_slot_usize).cloned() {
                                                                                    p
                                                                                } else {
                                                                                    let warn_key = format!("{:?}|{:?}|{}|{}", model_id, geo_id, mat_slot, material_table.len());
                                                    if missing_slot_warnings.insert(warn_key) {
                                                        // println!(
                                                        //     "WARN: material slot {} out of range for model={:?} name={:?} geo_id={:?} (materials={}). Using fallback.",
                                                        //     mat_slot, model_id, model_name, geo_id, material_table.len()
                                                        // );
                                                    }
                                                                                    // Heuristic: if this looks like a window family name and slot>0, treat as glass.
                                                                                    // We calculate geometry thinness later (in the bucket loop) for more accurate "mm" detection.
                                                                                    // Here we can only rely on names.
                                                                                    if mat_slot > 0 && (model_name.contains("窗") || model_name.contains("Window") || model_name.contains("Glass") || model_name.contains("玻璃")) {
                                                                                        glass_fallback.clone()
                                                                                    } else if mat_slot > 0 && model_name.contains("mm") {
                                                                                        // For "mm" names, we temporarily assign glass here, but we might revert it 
                                                                                        // or confirm it in the bucket loop based on geometry thickness.
                                                                                        // But wait, we push vertices based on this `mat_params`. 
                                                                                        // If we want geometry-based heuristic, we need to inspect the *current polygon's* contribution 
                                                                                        // or wait until we have the bucket. 
                                                                                        //
                                                                                        // PROBLEM: We are INSIDE the loop creating vertices. We don't have the full bucket AABB yet.
                                                                                        // However, we are triangulating a single polygon here (usually a quad or triangle).
                                                                                        // We can't judge "thinness" of a single polygon easily (it's always flat).
                                                                                        //
                                                                                        // APPROACH: We'll stick to the "name check" here for assigning the *initial* material params to vertices.
                                                                                        // But we know from the user issue that "mm" alone is ambiguous.
                                                                                        // If we assign "Glass" here, it goes into the bucket.
                                                                                        // If we assign "Default" here, it goes into the bucket.
                                                                                        // The "bucket loop" later (lines 945+) iterates over these already-filled buckets.
                                                                                        // If we made a mistake here, the vertices in the bucket have the WRONG material data baked in (color, opacity).
                                                                                        //
                                                                                        // BETTER APPROACH:
                                                                                        // 1. Assign a "Placeholder/Pending" material or just Default here.
                                                                                        // 2. In the bucket loop (where we have all verts for this slot), calculate AABB.
                                                                                        // 3. If "mm" + Thin -> Force Glass uniform/material on the RenderMesh.
                                                                                        //    BUT: Our Vertex struct *contains* the material data (per vertex). 
                                                                                        //    So we'd need to update the vertices in the bucket before creating the buffer.
                                                                                        
                                                                                        // Let's implement that: Use default here for "mm", then fixup in bucket loop.
                                                                                        default_params.clone()
                                                                                    } else {
                                                                                        default_params.clone()
                                                                                    }
                                                                                };

                                                                                let (bucket_verts, bucket_indices) =
                                                                                    buckets.entry(mat_slot).or_insert_with(|| (Vec::new(), Vec::new()));
                                                                                // Triangle Fan triangulation
                                                                                for k in 0..(count - 2) {
                                                                                     let idx0 = raw_indices[poly_start];
                                                                                     let idx1 = raw_indices[poly_start + k + 1];
                                                                                     let idx2 = raw_indices[poly_start + k + 2];
                                                                                     
                                                                                     let i0 = (if idx0 < 0 { !idx0 } else { idx0 }) as usize;
                                                                                     let i1 = (if idx1 < 0 { !idx1 } else { idx1 }) as usize;
                                                                                     let i2 = (if idx2 < 0 { !idx2 } else { idx2 }) as usize;
                                                                                     
                                                                                     if i0 < temp_positions.len() && i1 < temp_positions.len() && i2 < temp_positions.len() {
                                                                                         let p0 = temp_positions[i0];
                                                                                         let p1 = temp_positions[i1];
                                                                                         let p2 = temp_positions[i2];
                                                                                         
                                                                                        // Calculate Face Normal
                                                                                        let edge1 = p1 - p0;
                                                                                        let edge2 = p2 - p0;
                                                                                        let normal = edge1.cross(edge2).normalize_or_zero();
                                                                                        let normal_array = [normal.x, normal.y, normal.z];
                                                                                        
                                                                                        let uv0 = get_uv(poly_index, poly_vert_cursor, idx0);
                                                                                        let uv1 = get_uv(poly_index, poly_vert_cursor + k + 1, idx1);
                                                                                        let uv2 = get_uv(poly_index, poly_vert_cursor + k + 2, idx2);

                                                                                        // Push distinct vertices for each face corner to preserve hard edges
                                                                                        let base_index = bucket_verts.len() as u32;
                                                                                        
                                                                                        bucket_verts.push(Vertex {
                                                                                            position: [p0.x, p0.y, p0.z],
                                                                                            color: mat_params.color,
                                                                                            normal: normal_array,
                                                                                            uv: uv0,
                                                                                            material: mat_params.material,
                                                                                        });
                                                                                        bucket_verts.push(Vertex {
                                                                                            position: [p1.x, p1.y, p1.z],
                                                                                            color: mat_params.color,
                                                                                            normal: normal_array,
                                                                                            uv: uv1,
                                                                                            material: mat_params.material,
                                                                                        });
                                                                                        bucket_verts.push(Vertex {
                                                                                            position: [p2.x, p2.y, p2.z],
                                                                                            color: mat_params.color,
                                                                                            normal: normal_array,
                                                                                            uv: uv2,
                                                                                            material: mat_params.material,
                                                                                        });
                                                                                         
                                                                                         bucket_indices.push(base_index);
                                                                                         bucket_indices.push(base_index + 1);
                                                                                         bucket_indices.push(base_index + 2);
                                                                                     }
                                                                                }
                                                                                poly_start = i + 1;
                                                                                poly_index += 1;
                                                                                // For ByPolygonVertex, the cursor advances by the number of polygon vertices.
                                                                                // `count` includes the negative terminator entry, so subtract 1.
                                                                                poly_vert_cursor += count.saturating_sub(1);
                                                                            }
                                                                        }
                                                                        
                                                                        if DEBUG_FBX_MESH_MAT_SLOTS && !buckets.is_empty() {
                                                                            let mut slots: Vec<i32> = buckets.keys().copied().collect();
                                                                            slots.sort();
                                                                            println!(
                                                                                "FBX mesh material slots used: model={:?} name={:?} geo_id={:?} slots={:?}",
                                                                                model_id, model_name, geo_id, slots
                                                                            );
                                                                            // Print slot -> material name + opacity for quick diagnosis of "opaque windows"
                                                                            for s in &slots {
                                                                                let si = *s as usize;
                                                                                let name = material_ids
                                                                                    .get(si)
                                                                                    .and_then(|id| material_map.get(id))
                                                                                    .and_then(|m| m.node().attributes().get(1))
                                                                                    .and_then(|n| n.get_string())
                                                                                    .map(|n| n.split('\0').next().unwrap_or(n).trim().to_string());
                                                                                let opacity = material_table
                                                                                    .get(si)
                                                                                    .map(|mp| mp.material[2])
                                                                                    .unwrap_or(1.0);
                                                                                println!("  slot {} -> name={:?}, opacity={:.3}", s, name, opacity);
                                                                            }
                                                                        }

                                                                        for (slot, (mut verts, inds)) in buckets {
                                                                            if !verts.is_empty() {
                                                                                let si = slot.max(0) as usize;
                                                                                let mat_name = material_ids
                                                                                    .get(si)
                                                                                    .and_then(|id| material_map.get(id))
                                                                                    .and_then(|m| m.node().attributes().get(1))
                                                                                    .and_then(|n| n.get_string())
                                                                                    .map(|n| n.split('\0').next().unwrap_or(n).trim().to_string());
                                                                                
                                                                                // Re-determine params used (must match logic in loop above)
                                                                                let used_params = if let Some(p) = material_table.get(si).cloned() {
                                                                                    p
                                                                                } else {
                                                                                     // Fallback logic
                                                                                     let mut is_thin = false;
                                                                                     if !verts.is_empty() {
                                                                                        let mut min = Vec3::splat(f32::MAX);
                                                                                        let mut max = Vec3::splat(f32::MIN);
                                                                                        for v in &*verts {
                                                                                            let p = Vec3::new(v.position[0], v.position[1], v.position[2]);
                                                                                            min = min.min(p);
                                                                                            max = max.max(p);
                                                                                        }
                                                                                        let size = max - min;
                                                                                        let max_dim = size.max_element();
                                                                                        let min_dim = size.min_element();
                                                                                        // If thickness is less than 5% of max dimension, consider it thin (pane-like)
                                                                                        if max_dim > 0.001 && min_dim / max_dim < 0.05 {
                                                                                            is_thin = true;
                                                                                        }
                                                                                     }

                                                                                     if slot > 0 && (model_name.contains("窗") || model_name.contains("Window") || model_name.contains("Glass") || model_name.contains("玻璃") || (model_name.contains("mm") && is_thin)) {
                                                                                        glass_fallback.clone()
                                                                                    } else {
                                                                                        default_params.clone()
                                                                                    }
                                                                                };

                                                                                // Apply heuristic-determined material to all vertices in this bucket
                                                                                // This fixes cases where we initially guessed "Default" (for "mm") but now know it's "Glass" (because it's thin).
                                                                                if verts.first().map(|v| v.material) != Some(used_params.material) {
                                                                                    for v in verts.iter_mut() {
                                                                                        v.color = used_params.color;
                                                                                        v.material = used_params.material;
                                                                                    }
                                                                                }

                                                                                let opacity = used_params.material[2];
                                                                                let diffuse_texture = used_params.diffuse_texture;

                                                                                let label = Some(format!(
                                                                                    "model={:?} name={:?} geo_id={:?} slot={} mat={:?} opacity={:.3}",
                                                                                    model_id, model_name, geo_id, slot, mat_name, opacity
                                                                                ));
                                                                                
                                                                                // Chunking to avoid "Buffer size greater than maximum buffer size" panic
                                                                                const MAX_VERTS_PER_MESH: usize = 1_000_000;
                                                                                if verts.len() > MAX_VERTS_PER_MESH {
                                                                                    // println!("Large mesh detected ({:?} verts), splitting...", verts.len());
                                                                                    for (chunk_i, chunk) in verts.chunks(MAX_VERTS_PER_MESH).enumerate() {
                                                                                        let chunk_inds: Vec<u32> = (0..chunk.len() as u32).collect();
                                                                                        let chunk_label = label.as_ref().map(|l| format!("{} [part {}]", l, chunk_i));
                                                                                        view.add_mesh_with_label(device, &render_state.queue, chunk, &chunk_inds, chunk_label, diffuse_texture.clone());
                                                                                    }
                                                                                } else {
                                                                                    view.add_mesh_with_label(device, &render_state.queue, &verts, &inds, label, diffuse_texture);
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                            // 3. Compute Normals & Build Vertices (Removed - logic moved above)
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        ui.close_menu();
                    }
                    if ui.button("Export Selected...").clicked() {}
                    ui.separator();
                    if ui.button("Exit").clicked() {
                        ctx.send_viewport_cmd(egui::ViewportCommand::Close);
                    }
                });
                ui.menu_button("Edit", |ui| {
                    if ui.button("Undo").clicked() {}
                    if ui.button("Redo").clicked() {}
                    ui.separator();
                    if ui.button("Cut").clicked() {}
                    if ui.button("Copy").clicked() {}
                    if ui.button("Paste").clicked() {}
                    if ui.button("Delete").clicked() {}
                    ui.separator();
                    if ui.button("Select All").clicked() {}
                    if ui.button("Invert Selection").clicked() {}
                });
                ui.menu_button("View", |ui| {
                    if ui.button("Zoom Extents").clicked() {}
                    if ui.button("Zoom Selected").clicked() {}
                    ui.separator();
                    if ui.button("Pan").clicked() {}
                    if ui.button("Rotate").clicked() {}
                    ui.separator();
                    if ui.button("Wireframe").clicked() {}
                    if ui.button("Shaded").clicked() {}
                    if ui.button("Rendered").clicked() {}
                });
                ui.menu_button("Curve", |ui| {
                    ui.menu_button("Line", |ui| {
                        if ui.button("Single Line").clicked() {}
                        if ui.button("Polyline").clicked() {}
                    });
                    ui.menu_button("Arc", |ui| {
                         if ui.button("Center, Start, Angle").clicked() {}
                         if ui.button("Start, End, Point on Arc").clicked() {}
                    });
                    ui.menu_button("Circle", |ui| {
                        if ui.button("Center, Radius").clicked() {}
                        if ui.button("2 Points").clicked() {}
                        if ui.button("3 Points").clicked() {}
                    });
                    if ui.button("Ellipse").clicked() {}
                    ui.menu_button("Rectangle", |ui| {
                        if ui.button("Corner to Corner").clicked() {}
                        if ui.button("Center, Corner").clicked() {}
                    });
                    if ui.button("Polygon").clicked() {}
                });
                ui.menu_button("Surface", |ui| {
                    ui.menu_button("Plane", |ui| {
                        if ui.button("Corner to Corner").clicked() {}
                        if ui.button("3 Points").clicked() {}
                    });
                    ui.menu_button("Extrude Curve", |ui| {
                        if ui.button("Straight").clicked() {}
                    });
                    if ui.button("Loft").clicked() {}
                    if ui.button("Revolve").clicked() {}
                    if ui.button("Sweep 1 Rail").clicked() {}
                    if ui.button("Sweep 2 Rails").clicked() {}
                });
                ui.menu_button("Solid", |ui| {
                    if ui.button("Box").clicked() {}
                    if ui.button("Sphere").clicked() {}
                    if ui.button("Cylinder").clicked() {}
                    if ui.button("Cone").clicked() {}
                    ui.separator();
                    if ui.button("Boolean Union").clicked() {}
                    if ui.button("Boolean Difference").clicked() {}
                    if ui.button("Boolean Intersection").clicked() {}
                });
                ui.menu_button("Transform", |ui| {
                    if ui.button("Move").clicked() {}
                    if ui.button("Copy").clicked() {}
                    if ui.button("Rotate").clicked() {}
                    ui.menu_button("Scale", |ui| {
                         if ui.button("Scale 1D").clicked() {}
                         if ui.button("Scale 2D").clicked() {}
                         if ui.button("Scale 3D").clicked() {}
                    });
                    if ui.button("Mirror").clicked() {}
                    if ui.button("Array").clicked() {}
                });
                ui.menu_button("Tools", |ui| {
                    if ui.button("Options...").clicked() {}
                });
                ui.menu_button("Help", |ui| {
                    if ui.button("About").clicked() {}
                });
            });
        });

        // 2. Toolbar (Left) - Rhino-like Main with SVG Icons
        egui::SidePanel::left("left_panel")
            .resizable(false)
            .default_width(40.0)
            .show(ctx, |ui| {
                ui.vertical_centered(|ui| {
                    if icon_button(ui, "select", "Select").clicked() {}
                    if icon_button(ui, "point", "Point").clicked() {}
                    if icon_button(ui, "polyline", "Polyline").clicked() {}
                    if icon_button(ui, "curve", "Curve").clicked() {}
                    if icon_button(ui, "circle", "Circle").clicked() {}
                    if icon_button(ui, "ellipse", "Ellipse").clicked() {}
                    if icon_button(ui, "arc", "Arc").clicked() {}
                    if icon_button(ui, "rectangle", "Rectangle").clicked() {}
                    if icon_button(ui, "polygon", "Polygon").clicked() {}
                    ui.separator();
                    if icon_button(ui, "surface", "Surface").clicked() {}
                    if icon_button(ui, "box", "Box").clicked() {}
                    ui.separator();
                    if icon_button(ui, "extrude", "Extrude").clicked() {}
                    if icon_button(ui, "join", "Join").clicked() {}
                    if icon_button(ui, "explode", "Explode").clicked() {}
                    if icon_button(ui, "trim", "Trim").clicked() {}
                    if icon_button(ui, "split", "Split").clicked() {}
                    ui.separator();
                    if icon_button(ui, "group", "Group").clicked() {}
                    if icon_button(ui, "ungroup", "Ungroup").clicked() {}
                    ui.separator();
                    if icon_button(ui, "text", "Text").clicked() {}
                    if icon_button(ui, "dimension", "Dimension").clicked() {}
                });
            });

        // 3. Properties / Layers (Right)
        egui::SidePanel::right("right_panel")
            .resizable(true)
            .default_width(250.0)
            .show(ctx, |ui| {
                ui.heading("Properties");
                ui.separator();
                ui.label("Object Type: None");
                ui.label("Layer: Default");
                
                ui.separator();
                ui.collapsing("Layers", |ui| {
                    ui.label("Default (visible)");
                    ui.label("Layer 01");
                });
            });

        // 4. Command Line (Bottom)
        egui::TopBottomPanel::bottom("bottom_panel").show(ctx, |ui| {
            ui.horizontal(|ui| {
                ui.label("Command:");
                ui.text_edit_singleline(&mut self.command_input);
            });
            ui.separator();
            ui.horizontal(|ui| {
                ui.label("Console:");
                if ui.button("Clear").clicked() {
                    self.console_lines.clear();
                }
                if ui.button("Copy All").clicked() {
                    ui.output_mut(|o| o.copied_text = self.console_lines.join("\n"));
                }
            });
            egui::ScrollArea::vertical()
                .max_height(140.0)
                .auto_shrink([false; 2])
                .show(ui, |ui| {
                    for line in &self.console_lines {
                        ui.label(line);
                    }
                });
        });

        // Background Color Picker Window
        if self.show_bg_color_picker {
            let mut open = true;
            let mut close_clicked = false;
            
            egui::Window::new("Background Color")
                .open(&mut open)
                .resizable(false)
                .collapsible(false)
                .show(ctx, |ui| {
                    let mut lock = self.bim_view.lock().unwrap();
                    if let Some(view) = lock.as_mut() {
                        ui.horizontal(|ui| {
                            ui.label("Select Color:");
                            ui.color_edit_button_rgb(&mut view.background_color);
                        });
                        
                        ui.separator();
                        ui.horizontal(|ui| {
                            if ui.button("Close").clicked() {
                                close_clicked = true;
                            }
                        });
                    }
                });
            
            if close_clicked {
                open = false;
            }
            self.show_bg_color_picker = open;
        }

        // 5. Central Viewport (3D View)
        egui::CentralPanel::default().show(ctx, |ui| {
             let rect = ui.available_rect_before_wrap();
             
             // Update view size
             let mut pending_logs: Vec<String> = Vec::new();
             {
                 let mut lock = self.bim_view.lock().unwrap();
                 if let Some(view) = lock.as_mut() {
                    view.set_size(rect.width() as u32, rect.height() as u32);

                    // Zoom Box Logic
                    if self.is_zooming_box {
                        ui.output_mut(|o| o.cursor_icon = egui::CursorIcon::Crosshair);
                        
                        // We use a different ID for this interaction to avoid conflict, but we must
                        // ensure we are the only one interacting if possible.
                        let zoom_response = ui.interact(rect, ui.id().with("zoom_box"), egui::Sense::click_and_drag());

                        if zoom_response.drag_started() {
                            self.zoom_box_start = zoom_response.interact_pointer_pos();
                        }

                        if let Some(start) = self.zoom_box_start {
                            // If we have a start, we are in dragging mode. 
                            // Use pointer_pos instead of interact_pointer_pos to get position even if outside rect slightly?
                            // But interact_pointer_pos is safer for valid coordinates.
                            if let Some(curr) = ctx.pointer_interact_pos() {
                                let selection_rect = egui::Rect::from_two_pos(start, curr);
                                
                                // Use a foreground painter to ensure it's drawn on top of the 3D view
                                let painter = ctx.layer_painter(egui::LayerId::new(egui::Order::Foreground, egui::Id::new("zoom_box")));
                                
                                // Dashed line effect
                                let stroke = egui::Stroke::new(2.0, egui::Color32::WHITE);
                                let points = [
                                    selection_rect.min,
                                    egui::pos2(selection_rect.max.x, selection_rect.min.y),
                                    selection_rect.max,
                                    egui::pos2(selection_rect.min.x, selection_rect.max.y),
                                    selection_rect.min, // Close loop
                                ];
                                
                                let dash_len = 5.0;
                                let gap_len = 5.0;
                                
                                for i in 0..4 {
                                    let p1 = points[i];
                                    let p2 = points[i+1];
                                    let vec = p2 - p1;
                                    let len = vec.length();
                                    
                                    if len > 0.1 {
                                        let dir = vec / len;
                                        let count = (len / (dash_len + gap_len)).ceil() as i32;
                                        
                                        for j in 0..count {
                                            let t = j as f32 * (dash_len + gap_len);
                                            if t < len {
                                                let start_p = p1 + dir * t;
                                                let end_p = p1 + dir * (t + dash_len).min(len);
                                                painter.line_segment([start_p, end_p], stroke);
                                            }
                                        }
                                    }
                                }

                                painter.rect_filled(
                                    selection_rect,
                                    0.0,
                                    egui::Color32::from_rgba_unmultiplied(255, 255, 255, 20),
                                );

                                if zoom_response.drag_stopped() {
                                    if selection_rect.width() > 5.0 && selection_rect.height() > 5.0 {
                                        let vp = view.camera.build_view_projection_matrix();
                                        let inv_vp = vp.inverse();

                                        let get_world_point = |px: f32, py: f32| -> Option<Vec3> {
                                            let ndc_x = (px / rect.width()) * 2.0 - 1.0;
                                            let ndc_y = 1.0 - (py / rect.height()) * 2.0;
                                            let near = inv_vp * glam::Vec4::new(ndc_x, ndc_y, 0.0, 1.0);
                                            let far = inv_vp * glam::Vec4::new(ndc_x, ndc_y, 1.0, 1.0);

                                            let origin = near.truncate() / near.w;
                                            let end = far.truncate() / far.w;
                                            let dir = (end - origin).normalize_or_zero();

                                            let plane_center = view.camera.target;
                                            let plane_normal =
                                                (view.camera.eye - view.camera.target).normalize_or_zero();

                                            if plane_normal.length_squared() < 0.001 {
                                                return None;
                                            }

                                            let denom = dir.dot(plane_normal);
                                            if denom.abs() > 1e-6 {
                                                let t = (plane_center - origin).dot(plane_normal) / denom;
                                                if t >= 0.0 {
                                                    return Some(origin + dir * t);
                                                }
                                            }
                                            None
                                        };

                                        let p1 = get_world_point(
                                            selection_rect.min.x - rect.min.x,
                                            selection_rect.min.y - rect.min.y,
                                        );
                                        let p2 = get_world_point(
                                            selection_rect.max.x - rect.min.x,
                                            selection_rect.max.y - rect.min.y,
                                        );
                                        let p3 = get_world_point(
                                            selection_rect.max.x - rect.min.x,
                                            selection_rect.min.y - rect.min.y,
                                        );
                                        let p4 = get_world_point(
                                            selection_rect.min.x - rect.min.x,
                                            selection_rect.max.y - rect.min.y,
                                        );

                                        if let (Some(v1), Some(v2), Some(v3), Some(v4)) =
                                            (p1, p2, p3, p4)
                                        {
                                            let min = v1.min(v2).min(v3).min(v4);
                                            let max = v1.max(v2).max(v3).max(v4);
                                            view.camera_controller.zoom_to_fit(min, max);
                                        }
                                    }
                                    self.is_zooming_box = false;
                                    self.zoom_box_start = None;
                                }
                            }
                        }

                        if ctx.input(|i| i.key_pressed(egui::Key::Escape) || i.pointer.secondary_pressed())
                        {
                            self.is_zooming_box = false;
                            self.zoom_box_start = None;
                        }

                    } else {
                        // Handle Main Input only if NOT zooming box
                        let response = ui.interact(rect, ui.id(), egui::Sense::click_and_drag());
                        
                        if response.drag_started() {
                            view.camera_controller.save_view();
                        }
                        
                        // Game Mode Input
                        if view.camera_controller.mode == CameraMode::Game {
                                // request_focus is on memory() in recent egui versions or just use response.request_focus()
                                ui.memory_mut(|m| m.request_focus(response.id));
                            
                            let _dt = 0.016; // Assumption for now, better to get from frame
                            
                            if ctx.input(|i| i.key_down(egui::Key::W)) { view.camera_controller.move_forward(1.0); }
                            if ctx.input(|i| i.key_down(egui::Key::S)) { view.camera_controller.move_forward(-1.0); }
                            if ctx.input(|i| i.key_down(egui::Key::A)) { view.camera_controller.move_right(-1.0); }
                            if ctx.input(|i| i.key_down(egui::Key::D)) { view.camera_controller.move_right(1.0); }
                            if ctx.input(|i| i.key_down(egui::Key::Space)) { view.camera_controller.move_up(1.0); }
                            if ctx.input(|i| i.key_down(egui::Key::X)) { view.camera_controller.move_up(-1.0); } // X for down
                            if ctx.input(|i| i.key_down(egui::Key::E)) { view.camera_controller.move_up(1.0); }
                            if ctx.input(|i| i.key_down(egui::Key::Q)) { view.camera_controller.move_up(-1.0); }
                            
                            // Speed control
                            if ctx.input(|i| i.modifiers.shift) {
                                view.camera_controller.speed = 2.0;
                            } else {
                                view.camera_controller.speed = 0.5;
                            }
                            
                            // Mouse Look (only when dragging for now, or use locked cursor)
                            if response.dragged_by(egui::PointerButton::Secondary) || response.dragged_by(egui::PointerButton::Primary) {
                                let delta = response.drag_delta();
                                // Invert Y axis for Game Mode to match standard FPS look
                                view.camera_controller.rotate(delta.x * 0.005, -delta.y * 0.005);
                            }
                            
                            // Continuous repaint for game loop
                            ui.ctx().request_repaint();
                        } else {
                            // Orbit Mode (Existing)
                            if response.dragged_by(egui::PointerButton::Secondary) {
                                let delta = response.drag_delta();
                                view.camera_controller.rotate(delta.x * 0.01, delta.y * 0.01);
                            }
                            
                            if response.dragged_by(egui::PointerButton::Middle) {
                                let delta = response.drag_delta();
                                view.camera_controller.pan(delta.x, delta.y);
                            }
                        }
                        
                        // Zoom
                        if response.hovered() {
                            let scroll = ctx.input(|i| i.raw_scroll_delta.y);
                            if scroll != 0.0 {
                                view.camera_controller.zoom(scroll * 0.002);
                            }
                        }

                        // Double Click Middle Mouse for Context Menu
                        if response.double_clicked_by(egui::PointerButton::Middle) {
                            self.context_menu_pos = response.interact_pointer_pos();
                        } else if response.clicked() || response.dragged() {
                            // Close if clicked elsewhere
                            self.context_menu_pos = None;
                        }

                        // Left click: pick mesh (AABB ray test)
                        if response.clicked_by(egui::PointerButton::Primary) {
                            if let Some(pos) = response.interact_pointer_pos() {
                                let local_x = (pos.x - rect.min.x).max(0.0);
                                let local_y = (pos.y - rect.min.y).max(0.0);
                                if let Some(idx) = view.pick_mesh(local_x, local_y) {
                                    self.selected_mesh = Some(idx);
                                    view.set_selected_mesh(Some(idx));
                                   let m = &view.meshes[idx];
                                   let msg = format!(
                                       "Picked mesh #{idx}: label={:?} translucent={} aabb=({:?}..{:?})",
                                       m.label, m.is_translucent, m.aabb.0, m.aabb.1
                                   );
                                   // println!("{msg}");
                                   pending_logs.push(msg);
                                } else {
                                    view.set_selected_mesh(None);
                                    let msg = format!("Pick miss at ({local_x:.1},{local_y:.1})");
                                    pending_logs.push(msg);
                                }
                           }
                       }
                    } // End of !is_zooming_box

                    // Draw Context Menu
                     if let Some(pos) = self.context_menu_pos {
                         let mut close_menu = false;
                         egui::Area::new("context_menu".into())
                             .fixed_pos(pos)
                             .order(egui::Order::Foreground)
                             .show(ctx, |ui| {
                                 egui::Frame::menu(ui.style()).show(ui, |ui| {
                                     ui.set_max_width(150.0);
                                    if ui.button("Zoom Extents").clicked() {
                                        let mut min = Vec3::splat(f32::MAX);
                                        let mut max = Vec3::splat(f32::MIN);
                                        let mut any = false;
                                        for mesh in &view.meshes {
                                            min = min.min(mesh.aabb.0);
                                            max = max.max(mesh.aabb.1);
                                            any = true;
                                        }
                                        if any {
                                            view.camera_controller.zoom_to_fit(min, max);
                                        } else {
                                            view.camera_controller.zoom_extents();
                                        }
                                        close_menu = true;
                                    }
                                    // Zoom to Selected Object
                                    if let Some(selected_idx) = self.selected_mesh {
                                        if selected_idx < view.meshes.len() {
                                            if ui.button("Zoom to Selected Object").clicked() {
                                                let mesh = &view.meshes[selected_idx];
                                                view.camera_controller.zoom_to_fit(mesh.aabb.0, mesh.aabb.1);
                                                close_menu = true;
                                            }
                                        } else {
                                            ui.add_enabled(false, egui::Button::new("Zoom to Selected Object"));
                                        }
                                    } else {
                                        ui.add_enabled(false, egui::Button::new("Zoom to Selected Object"));
                                    }
                                    if ui.button("Zoom Random Object").clicked() {
                                        if !view.meshes.is_empty() {
                                            // Simple pseudo-random
                                            let now = std::time::SystemTime::now().duration_since(std::time::UNIX_EPOCH).unwrap().as_millis();
                                            let idx = (now as usize) % view.meshes.len();
                                            let mesh = &view.meshes[idx];
                                            view.camera_controller.zoom_to_fit(mesh.aabb.0, mesh.aabb.1);
                                        }
                                        close_menu = true;
                                    }
                                    if ui.button("Zoom to Box").clicked() {
                                        self.is_zooming_box = true;
                                        close_menu = true;
                                    }
                                    ui.separator();
                                    if ui.button("Previous Viewport Range").clicked() {
                                        view.camera_controller.undo_view();
                                        close_menu = true;
                                    }
                                 });
                             });
                         
                         if close_menu {
                             self.context_menu_pos = None;
                         }
                         
                        // Close if clicking outside area (handled somewhat by area, but explicit check is good)
                        // Note: 'response' is defined in the else block above, but not here.
                        // We rely on main interact loop to clear if clicked elsewhere.
                     }
                     
                     // --- OVERLAYS ---
                     // Top-Left: Viewport Controls
                     let tl_pos = rect.min + egui::vec2(10.0, 10.0);
                     egui::Area::new("vp_controls_area".into())
                        .fixed_pos(tl_pos)
                        .show(ctx, |ui| {
                            egui::Frame::popup(ui.style())
                                .shadow(egui::epaint::Shadow::default())
                                .show(ui, |ui| {
                                    ui.menu_button("Viewport", |ui| {
                                        ui.menu_button("Layout", |ui| {
                                            if ui.button("Single").clicked() {} 
                                            if ui.button("Quad").clicked() {}
                                            if ui.button("Split").clicked() {}
                                        });
                                        ui.separator();
                                        
                                        ui.menu_button("Projection", |ui| {
                                            if ui.radio_value(&mut view.camera.projection_type, ProjectionType::Perspective, "Perspective").clicked() {}
                                            if ui.radio_value(&mut view.camera.projection_type, ProjectionType::Orthographic, "Orthographic").clicked() {}
                                        });
                                        ui.separator();
                                        
                                        ui.menu_button("Standard Views", |ui| {
                                            if ui.button("Top").clicked() { view.camera_controller.set_view(0.0, std::f32::consts::FRAC_PI_2 - 0.001); }
                                            if ui.button("Bottom").clicked() { view.camera_controller.set_view(0.0, -std::f32::consts::FRAC_PI_2 + 0.001); }
                                            if ui.button("Front").clicked() { view.camera_controller.set_view(-std::f32::consts::FRAC_PI_2, 0.0); }
                                            if ui.button("Back").clicked() { view.camera_controller.set_view(std::f32::consts::FRAC_PI_2, 0.0); }
                                            if ui.button("Left").clicked() { view.camera_controller.set_view(std::f32::consts::PI, 0.0); }
                                            if ui.button("Right").clicked() { view.camera_controller.set_view(0.0, 0.0); }
                                        });
                                        ui.separator();

                                        ui.menu_button("Display Mode", |ui| {
                                            if ui.radio(view.display_mode == DisplayMode::Wireframe, "Wireframe").clicked() {
                                                view.display_mode = DisplayMode::Wireframe;
                                                ui.close_menu();
                                            }
                                            if ui.radio(view.display_mode == DisplayMode::Shaded, "Shaded").clicked() {
                                                view.display_mode = DisplayMode::Shaded;
                                                ui.close_menu();
                                            }
                                            if ui.radio(view.display_mode == DisplayMode::MaterialPreview, "Material Preview").clicked() {
                                                view.display_mode = DisplayMode::MaterialPreview;
                                                ui.close_menu();
                                            }
                                            if ui.radio(view.display_mode == DisplayMode::XRay, "X-Ray").clicked() {
                                                view.display_mode = DisplayMode::XRay;
                                                ui.close_menu();
                                            }
                                            if ui.radio(view.display_mode == DisplayMode::Translucent, "Translucent").clicked() {
                                                view.display_mode = DisplayMode::Translucent;
                                                ui.close_menu();
                                            }
                                            if ui.radio(view.display_mode == DisplayMode::Artistic, "Artistic").clicked() {
                                                view.display_mode = DisplayMode::Artistic;
                                                ui.close_menu();
                                            }
                                        });
                                        ui.separator();
                                        
                                        ui.menu_button("Camera Mode", |ui| {
                                            let mode = view.camera_controller.mode;
                                            if ui.radio(mode == CameraMode::Orbit, "Orbit (Default)").clicked() {
                                                view.camera_controller.set_mode(CameraMode::Orbit);
                                            }
                                            if ui.radio(mode == CameraMode::Game, "Game (WASD)").clicked() {
                                                view.camera_controller.set_mode(CameraMode::Game);
                                            }
                                        });

                                        ui.menu_button("Environment", |ui| {
                                            if ui.checkbox(&mut view.show_grid, "Grid").clicked() {}
                                            if ui.checkbox(&mut view.show_axis, "Axis").clicked() {}
                                        });

                                        ui.separator();
                                        if ui.button("Background Color...").clicked() {
                                            self.show_bg_color_picker = true;
                                            ui.close_menu();
                                        }
                                    });
                                });
                        });

                     // Top-Right: ViewCube
                     let tr_pos = rect.min + egui::vec2(rect.width() - 120.0, 10.0);
                     egui::Area::new("viewcube_area".into())
                        .fixed_pos(tr_pos)
                        .show(ctx, |ui| {
                            let size = 100.0;
                            let (response, painter) = ui.allocate_painter(egui::vec2(size, size), egui::Sense::click());
                            
                            // Draw Isometric Cube
                            let center = response.rect.center();
                            let r = size * 0.35;
                            
                            // Vertices relative to center
                            // 30 deg increments. 
                            // 0 deg is right. 
                            // 30 (bottom right), 90 (bottom), 150 (bottom left), 210 (top left), 270 (top), 330 (top right)
                            // But usually isometric is:
                            // Top face: diamond
                            // Front face: rectangle-ish
                            // Right face: rectangle-ish
                            
                            // Angles for hexagon vertices (starting from right, ccw)
                            // 30, 90, 150, 210, 270, 330
                            let angle_offsets = [330.0, 270.0, 210.0, 150.0, 90.0, 30.0];
                            let rads: Vec<f32> = angle_offsets.iter().map(|d: &f32| d.to_radians()).collect();
                            
                            let mut points = Vec::new();
                            for rad in &rads {
                                points.push(center + egui::vec2(r * rad.cos(), r * rad.sin())); // Y is down in screen coords, so sin is ok
                            }
                            // points: 0(TR), 1(T), 2(TL), 3(BL), 4(B), 5(BR)
                            
                            let c = center;
                            
                            // Top Face (0, 1, 2, C)
                            let top_poly = vec![points[0], points[1], points[2], c];
                            // Left Face (2, 3, 4, C) - actually Front Left
                            let left_poly = vec![points[2], points[3], points[4], c];
                            // Right Face (4, 5, 0, C) - actually Front Right
                            let right_poly = vec![points[4], points[5], points[0], c];
                            
                            // Colors
                            let color_top = egui::Color32::from_rgb(200, 200, 200);
                            let color_left = egui::Color32::from_rgb(150, 150, 150);
                            let color_right = egui::Color32::from_rgb(100, 100, 100);
                            
                            let stroke = egui::Stroke::new(1.0, egui::Color32::BLACK);
                            
                            // Draw
                            painter.add(egui::Shape::convex_polygon(top_poly.clone(), color_top, stroke));
                            painter.add(egui::Shape::convex_polygon(left_poly.clone(), color_left, stroke));
                            painter.add(egui::Shape::convex_polygon(right_poly.clone(), color_right, stroke));
                            
                            // Text labels
                            painter.text(c + egui::vec2(0.0, -r*0.5), egui::Align2::CENTER_CENTER, "TOP", egui::FontId::proportional(10.0), egui::Color32::BLACK);
                            painter.text(c + egui::vec2(-r*0.5, r*0.5), egui::Align2::CENTER_CENTER, "FRONT", egui::FontId::proportional(10.0), egui::Color32::BLACK);
                            painter.text(c + egui::vec2(r*0.5, r*0.5), egui::Align2::CENTER_CENTER, "RIGHT", egui::FontId::proportional(10.0), egui::Color32::WHITE);
                            
                            // Interaction
                            if response.clicked() {
                                if let Some(pos) = response.interact_pointer_pos() {
                                    // Simple hit test based on centers
                                    // Top center
                                    let top_c = c + egui::vec2(0.0, -r*0.5);
                                    let left_c = c + egui::vec2(-r*0.5, r*0.5);
                                    let right_c = c + egui::vec2(r*0.5, r*0.5);
                                    
                                    let d_top = pos.distance(top_c);
                                    let d_left = pos.distance(left_c);
                                    let d_right = pos.distance(right_c);
                                    
                                    if d_top < d_left && d_top < d_right {
                                        view.camera_controller.set_view(0.0, std::f32::consts::FRAC_PI_2 - 0.001); // Top
                                    } else if d_left < d_top && d_left < d_right {
                                        view.camera_controller.set_view(-std::f32::consts::FRAC_PI_2, 0.0); // Front (Left in our poly but usually Front)
                                    } else {
                                        view.camera_controller.set_view(0.0, 0.0); // Right
                                    }
                                }
                            }
                            
                            // Corner buttons (small circles)
                             let corners = [
                                (points[1], "Back", std::f32::consts::FRAC_PI_2, 0.0), // Top corner -> Back? No, Top-Back
                                (points[4], "Bottom", 0.0, -std::f32::consts::FRAC_PI_2 + 0.001), // Bottom corner
                            ];
                            
                            for (p, _label, _yaw, _pitch) in corners {
                                painter.circle_filled(p, 4.0, egui::Color32::WHITE);
                                painter.circle_stroke(p, 4.0, egui::Stroke::new(1.0, egui::Color32::BLACK));
                                
                                // Need actual buttons or hit test?
                                // Let's use invisible buttons for now if we want robust input, but painter is immediate.
                                // We'll stick to face clicking for now as primary req.
                            }

                        });
                 }
             }

             // Flush console messages after releasing the view lock (avoid borrowing `self` mutably while holding the mutex guard).
             for msg in pending_logs.drain(..) {
                 self.push_console(msg);
             }

             let callback = BimViewCallback {
                 view: self.bim_view.clone(),
             };
             
             let cb = egui_wgpu::Callback::new_paint_callback(
                 rect,
                 callback,
             );
             ui.painter().add(cb);
        });
    }
}
