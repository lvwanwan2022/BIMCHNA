use crate::scene::{Camera, CameraController, CameraUniform};
use wgpu::util::DeviceExt;

#[repr(C)]
#[derive(Copy, Clone, Debug, bytemuck::Pod, bytemuck::Zeroable)]
pub struct Vertex {
    pub position: [f32; 3],
    pub color: [f32; 3],
    pub normal: [f32; 3],
    pub uv: [f32; 2],
    pub material: [f32; 4], // x: metallic, y: roughness, z: opacity, w: emission
}

pub struct RenderMesh {
    pub vertex_buffer: wgpu::Buffer,
    pub index_buffer: wgpu::Buffer,
    pub index_count: u32,
    pub aabb: (glam::Vec3, glam::Vec3),
    pub center: glam::Vec3,
    pub is_translucent: bool,
    pub label: Option<String>,
    pub texture_path: Option<String>,
    // CPU-side geometry for accurate picking (ray-triangle).
    pub pick_positions: Vec<glam::Vec3>,
    pub pick_indices: Vec<u32>,
}

pub struct BimView {
    // 3D Render State
    pbr_pipeline: wgpu::RenderPipeline,
    shaded_pipeline: wgpu::RenderPipeline,
    wireframe_pipeline: wgpu::RenderPipeline,
    selection_pipeline: wgpu::RenderPipeline,
    line_pipeline: wgpu::RenderPipeline,
    translucent_pipeline: wgpu::RenderPipeline,
    
    // Offscreen / Blit
    blit_pipeline: wgpu::RenderPipeline,
    render_texture: Option<wgpu::Texture>,
    render_view: Option<wgpu::TextureView>,
    depth_texture: Option<wgpu::Texture>,
    depth_view: Option<wgpu::TextureView>,
    sampler: wgpu::Sampler,
    blit_bind_group: Option<wgpu::BindGroup>,
    blit_bind_group_layout: wgpu::BindGroupLayout,
    
    // Scene Data
    pub camera: Camera,
    pub camera_controller: CameraController,
    camera_uniform: CameraUniform,
    camera_buffer: wgpu::Buffer,
    camera_bind_group: wgpu::BindGroup,
    
    // Texture State
    pub texture_bind_group_layout: wgpu::BindGroupLayout,
    pub default_texture_bind_group: wgpu::BindGroup,
    pub texture_cache: std::collections::HashMap<String, wgpu::BindGroup>,
    pub white_texture: wgpu::Texture,

    // Geometry
    grid_buffer: wgpu::Buffer,
    grid_count: u32,
    axis_buffer: wgpu::Buffer,
    axis_count: u32,

    pub meshes: Vec<RenderMesh>,
    
    // State
    width: u32,
    height: u32,
    target_width: u32,
    target_height: u32,
    
    pub show_grid: bool,
    pub show_axis: bool,
    pub background_color: [f32; 3],
    pub display_mode: DisplayMode,
    pub selected_mesh: Option<usize>,
}

#[derive(Debug, Clone, Copy, PartialEq, Eq)]
pub enum DisplayMode {
    Wireframe,
    Shaded,
    MaterialPreview, // PBR material preview
    XRay,            // For now same as Shaded or maybe simple blend
    Translucent,
    Artistic,
}

impl BimView {
    pub fn new(device: &wgpu::Device, queue: &wgpu::Queue, format: wgpu::TextureFormat) -> Self {
        let shader = device.create_shader_module(wgpu::ShaderModuleDescriptor {
            label: Some("Shader"),
            source: wgpu::ShaderSource::Wgsl(include_str!("shader.wgsl").into()),
        });
        
        let blit_shader = device.create_shader_module(wgpu::ShaderModuleDescriptor {
            label: Some("Blit Shader"),
            source: wgpu::ShaderSource::Wgsl(include_str!("blit.wgsl").into()),
        });

        // --- Camera & Uniforms ---
        let mut camera_controller = CameraController::new(10.0);
        let mut camera = Camera {
            eye: (0.0, 0.0, 0.0).into(), // will be set by controller
            target: (0.0, 0.0, 0.0).into(),
            up: glam::Vec3::Z,
            aspect: 1.0,
            fovy: std::f32::consts::FRAC_PI_4,
            znear: 0.1,
            zfar: 1000.0,
            projection_type: crate::scene::ProjectionType::Perspective,
            ortho_width: 10.0,
        };
        camera_controller.update_camera(&mut camera);

        let mut camera_uniform = CameraUniform::new();
        camera_uniform.update_view_proj(&camera);

        let camera_buffer = device.create_buffer_init(&wgpu::util::BufferInitDescriptor {
            label: Some("Camera Buffer"),
            contents: bytemuck::cast_slice(&[camera_uniform]),
            usage: wgpu::BufferUsages::UNIFORM | wgpu::BufferUsages::COPY_DST,
        });

        let camera_bind_group_layout = device.create_bind_group_layout(&wgpu::BindGroupLayoutDescriptor {
            entries: &[wgpu::BindGroupLayoutEntry {
                binding: 0,
                visibility: wgpu::ShaderStages::VERTEX | wgpu::ShaderStages::FRAGMENT,
                ty: wgpu::BindingType::Buffer {
                    ty: wgpu::BufferBindingType::Uniform,
                    has_dynamic_offset: false,
                    min_binding_size: None,
                },
                count: None,
            }],
            label: Some("camera_bind_group_layout"),
        });

        let camera_bind_group = device.create_bind_group(&wgpu::BindGroupDescriptor {
            layout: &camera_bind_group_layout,
            entries: &[wgpu::BindGroupEntry {
                binding: 0,
                resource: camera_buffer.as_entire_binding(),
            }],
            label: Some("camera_bind_group"),
        });

        // --- Texture Support ---
        let texture_bind_group_layout = device.create_bind_group_layout(&wgpu::BindGroupLayoutDescriptor {
            entries: &[
                // Binding 0: Texture
                wgpu::BindGroupLayoutEntry {
                    binding: 0,
                    visibility: wgpu::ShaderStages::FRAGMENT,
                    ty: wgpu::BindingType::Texture {
                        multisampled: false,
                        view_dimension: wgpu::TextureViewDimension::D2,
                        sample_type: wgpu::TextureSampleType::Float { filterable: true },
                    },
                    count: None,
                },
                // Binding 1: Sampler
                wgpu::BindGroupLayoutEntry {
                    binding: 1,
                    visibility: wgpu::ShaderStages::FRAGMENT,
                    ty: wgpu::BindingType::Sampler(wgpu::SamplerBindingType::Filtering),
                    count: None,
                },
            ],
            label: Some("texture_bind_group_layout"),
        });

        // Create default white texture
        let white_texture = device.create_texture_with_data(
            queue,
            &wgpu::TextureDescriptor {
                label: Some("White Texture"),
                size: wgpu::Extent3d { width: 1, height: 1, depth_or_array_layers: 1 },
                mip_level_count: 1,
                sample_count: 1,
                dimension: wgpu::TextureDimension::D2,
                format: wgpu::TextureFormat::Rgba8UnormSrgb,
                usage: wgpu::TextureUsages::TEXTURE_BINDING | wgpu::TextureUsages::COPY_DST,
                view_formats: &[],
            },
            wgpu::util::TextureDataOrder::LayerMajor,
            &[255, 255, 255, 255],
        );
        let white_view = white_texture.create_view(&wgpu::TextureViewDescriptor::default());
        let default_sampler = device.create_sampler(&wgpu::SamplerDescriptor {
            address_mode_u: wgpu::AddressMode::Repeat,
            address_mode_v: wgpu::AddressMode::Repeat,
            mag_filter: wgpu::FilterMode::Linear,
            min_filter: wgpu::FilterMode::Linear,
            ..Default::default()
        });

        let default_texture_bind_group = device.create_bind_group(&wgpu::BindGroupDescriptor {
            layout: &texture_bind_group_layout,
            entries: &[
                wgpu::BindGroupEntry {
                    binding: 0,
                    resource: wgpu::BindingResource::TextureView(&white_view),
                },
                wgpu::BindGroupEntry {
                    binding: 1,
                    resource: wgpu::BindingResource::Sampler(&default_sampler),
                },
            ],
            label: Some("default_texture_bind_group"),
        });

        // --- Pipelines ---
        let render_pipeline_layout = device.create_pipeline_layout(&wgpu::PipelineLayoutDescriptor {
            label: Some("Render Pipeline Layout"),
            bind_group_layouts: &[&camera_bind_group_layout, &texture_bind_group_layout],
            push_constant_ranges: &[],
        });

        let pbr_pipeline = device.create_render_pipeline(&wgpu::RenderPipelineDescriptor {
            label: Some("PBR Render Pipeline"),
            layout: Some(&render_pipeline_layout),
            vertex: wgpu::VertexState {
                module: &shader,
                entry_point: "vs_main",
                buffers: &[wgpu::VertexBufferLayout {
                    array_stride: std::mem::size_of::<Vertex>() as wgpu::BufferAddress,
                    step_mode: wgpu::VertexStepMode::Vertex,
                    attributes: &[
                        wgpu::VertexAttribute { offset: 0, shader_location: 0, format: wgpu::VertexFormat::Float32x3 },
                        wgpu::VertexAttribute { offset: 12, shader_location: 1, format: wgpu::VertexFormat::Float32x3 },
                        wgpu::VertexAttribute { offset: 24, shader_location: 2, format: wgpu::VertexFormat::Float32x3 },
                        wgpu::VertexAttribute { offset: 36, shader_location: 3, format: wgpu::VertexFormat::Float32x2 },
                        wgpu::VertexAttribute { offset: 44, shader_location: 4, format: wgpu::VertexFormat::Float32x4 },
                    ],
                }],
                compilation_options: wgpu::PipelineCompilationOptions::default(),
            },
            fragment: Some(wgpu::FragmentState {
                module: &shader,
                entry_point: "fs_main",
                targets: &[Some(wgpu::ColorTargetState {
                    format: wgpu::TextureFormat::Rgba8UnormSrgb, // Offscreen target format
                    blend: Some(wgpu::BlendState::ALPHA_BLENDING),
                    write_mask: wgpu::ColorWrites::ALL,
                })],
                compilation_options: wgpu::PipelineCompilationOptions::default(),
            }),
            primitive: wgpu::PrimitiveState {
                topology: wgpu::PrimitiveTopology::TriangleList,
                cull_mode: Some(wgpu::Face::Back), // Enable backface culling for solid objects
                polygon_mode: wgpu::PolygonMode::Fill,
                ..Default::default()
            },
            depth_stencil: Some(wgpu::DepthStencilState {
                format: wgpu::TextureFormat::Depth32Float,
                depth_write_enabled: true,
                depth_compare: wgpu::CompareFunction::Less,
                stencil: wgpu::StencilState::default(),
                bias: wgpu::DepthBiasState::default(),
            }),
            multisample: wgpu::MultisampleState::default(),
            multiview: None,
            cache: None,
        });

        // Shaded Pipeline (simple, non-PBR)
        let shaded_pipeline = device.create_render_pipeline(&wgpu::RenderPipelineDescriptor {
            label: Some("Shaded Render Pipeline"),
            layout: Some(&render_pipeline_layout),
            vertex: wgpu::VertexState {
                module: &shader,
                entry_point: "vs_main",
                buffers: &[wgpu::VertexBufferLayout {
                    array_stride: std::mem::size_of::<Vertex>() as wgpu::BufferAddress,
                    step_mode: wgpu::VertexStepMode::Vertex,
                    attributes: &[
                        wgpu::VertexAttribute { offset: 0, shader_location: 0, format: wgpu::VertexFormat::Float32x3 },
                        wgpu::VertexAttribute { offset: 12, shader_location: 1, format: wgpu::VertexFormat::Float32x3 },
                        wgpu::VertexAttribute { offset: 24, shader_location: 2, format: wgpu::VertexFormat::Float32x3 },
                        wgpu::VertexAttribute { offset: 36, shader_location: 3, format: wgpu::VertexFormat::Float32x2 },
                        wgpu::VertexAttribute { offset: 44, shader_location: 4, format: wgpu::VertexFormat::Float32x4 },
                    ],
                }],
                compilation_options: wgpu::PipelineCompilationOptions::default(),
            },
            fragment: Some(wgpu::FragmentState {
                module: &shader,
                entry_point: "fs_shaded",
                targets: &[Some(wgpu::ColorTargetState {
                    format: wgpu::TextureFormat::Rgba8UnormSrgb,
                    blend: Some(wgpu::BlendState::ALPHA_BLENDING),
                    write_mask: wgpu::ColorWrites::ALL,
                })],
                compilation_options: wgpu::PipelineCompilationOptions::default(),
            }),
            primitive: wgpu::PrimitiveState {
                topology: wgpu::PrimitiveTopology::TriangleList,
                cull_mode: Some(wgpu::Face::Back),
                polygon_mode: wgpu::PolygonMode::Fill,
                ..Default::default()
            },
            depth_stencil: Some(wgpu::DepthStencilState {
                format: wgpu::TextureFormat::Depth32Float,
                depth_write_enabled: true,
                depth_compare: wgpu::CompareFunction::Less,
                stencil: wgpu::StencilState::default(),
                bias: wgpu::DepthBiasState::default(),
            }),
            multisample: wgpu::MultisampleState::default(),
            multiview: None,
            cache: None,
        });

        // Wireframe Pipeline
        let wireframe_pipeline = device.create_render_pipeline(&wgpu::RenderPipelineDescriptor {
            label: Some("Wireframe Pipeline"),
            layout: Some(&render_pipeline_layout),
            vertex: wgpu::VertexState {
                module: &shader,
                entry_point: "vs_main",
                buffers: &[wgpu::VertexBufferLayout {
                    array_stride: std::mem::size_of::<Vertex>() as wgpu::BufferAddress,
                    step_mode: wgpu::VertexStepMode::Vertex,
                    attributes: &[
                        wgpu::VertexAttribute { offset: 0, shader_location: 0, format: wgpu::VertexFormat::Float32x3 },
                        wgpu::VertexAttribute { offset: 12, shader_location: 1, format: wgpu::VertexFormat::Float32x3 },
                        wgpu::VertexAttribute { offset: 24, shader_location: 2, format: wgpu::VertexFormat::Float32x3 },
                        wgpu::VertexAttribute { offset: 36, shader_location: 3, format: wgpu::VertexFormat::Float32x2 },
                        wgpu::VertexAttribute { offset: 44, shader_location: 4, format: wgpu::VertexFormat::Float32x4 },
                    ],
                }],
                compilation_options: wgpu::PipelineCompilationOptions::default(),
            },
            fragment: Some(wgpu::FragmentState {
                module: &shader,
                entry_point: "fs_wireframe", // Use specific entry point for wireframes
                targets: &[Some(wgpu::ColorTargetState {
                    format: wgpu::TextureFormat::Rgba8UnormSrgb,
                    blend: Some(wgpu::BlendState::ALPHA_BLENDING),
                    write_mask: wgpu::ColorWrites::ALL,
                })],
                compilation_options: wgpu::PipelineCompilationOptions::default(),
            }),
            primitive: wgpu::PrimitiveState {
                topology: wgpu::PrimitiveTopology::TriangleList,
                cull_mode: Some(wgpu::Face::Back),
                polygon_mode: wgpu::PolygonMode::Line, // Wireframe
                ..Default::default()
            },
            depth_stencil: Some(wgpu::DepthStencilState {
                format: wgpu::TextureFormat::Depth32Float,
                depth_write_enabled: true,
                depth_compare: wgpu::CompareFunction::Less,
                stencil: wgpu::StencilState::default(),
                // Add depth bias to bring lines forward
                bias: wgpu::DepthBiasState {
                    constant: -1,
                    slope_scale: -0.5,
                    clamp: 0.0,
                },
            }),
            multisample: wgpu::MultisampleState::default(),
            multiview: None,
            cache: None,
        });

        // Selection Pipeline (wireframe + constant color)
        let selection_pipeline = device.create_render_pipeline(&wgpu::RenderPipelineDescriptor {
            label: Some("Selection Pipeline"),
            layout: Some(&render_pipeline_layout),
            vertex: wgpu::VertexState {
                module: &shader,
                entry_point: "vs_main",
                buffers: &[wgpu::VertexBufferLayout {
                    array_stride: std::mem::size_of::<Vertex>() as wgpu::BufferAddress,
                    step_mode: wgpu::VertexStepMode::Vertex,
                    attributes: &[
                        wgpu::VertexAttribute { offset: 0, shader_location: 0, format: wgpu::VertexFormat::Float32x3 },
                        wgpu::VertexAttribute { offset: 12, shader_location: 1, format: wgpu::VertexFormat::Float32x3 },
                        wgpu::VertexAttribute { offset: 24, shader_location: 2, format: wgpu::VertexFormat::Float32x3 },
                        wgpu::VertexAttribute { offset: 36, shader_location: 3, format: wgpu::VertexFormat::Float32x2 },
                        wgpu::VertexAttribute { offset: 44, shader_location: 4, format: wgpu::VertexFormat::Float32x4 },
                    ],
                }],
                compilation_options: wgpu::PipelineCompilationOptions::default(),
            },
            fragment: Some(wgpu::FragmentState {
                module: &shader,
                entry_point: "fs_selection",
                targets: &[Some(wgpu::ColorTargetState {
                    format: wgpu::TextureFormat::Rgba8UnormSrgb,
                    blend: Some(wgpu::BlendState::ALPHA_BLENDING),
                    write_mask: wgpu::ColorWrites::ALL,
                })],
                compilation_options: wgpu::PipelineCompilationOptions::default(),
            }),
            primitive: wgpu::PrimitiveState {
                topology: wgpu::PrimitiveTopology::TriangleList,
                cull_mode: Some(wgpu::Face::Back),
                polygon_mode: wgpu::PolygonMode::Line,
                ..Default::default()
            },
            depth_stencil: Some(wgpu::DepthStencilState {
                format: wgpu::TextureFormat::Depth32Float,
                depth_write_enabled: false,
                depth_compare: wgpu::CompareFunction::LessEqual,
                stencil: wgpu::StencilState::default(),
                // Bring the outline forward a bit to avoid z-fighting
                bias: wgpu::DepthBiasState {
                    constant: -2,
                    slope_scale: -1.0,
                    clamp: 0.0,
                },
            }),
            multisample: wgpu::MultisampleState::default(),
            multiview: None,
            cache: None,
        });

        // Translucent Pipeline (No Depth Write)
        let translucent_pipeline = device.create_render_pipeline(&wgpu::RenderPipelineDescriptor {
            label: Some("Translucent Pipeline"),
            layout: Some(&render_pipeline_layout),
            vertex: wgpu::VertexState {
                module: &shader,
                entry_point: "vs_main",
                buffers: &[wgpu::VertexBufferLayout {
                    array_stride: std::mem::size_of::<Vertex>() as wgpu::BufferAddress,
                    step_mode: wgpu::VertexStepMode::Vertex,
                    attributes: &[
                        wgpu::VertexAttribute { offset: 0, shader_location: 0, format: wgpu::VertexFormat::Float32x3 },
                        wgpu::VertexAttribute { offset: 12, shader_location: 1, format: wgpu::VertexFormat::Float32x3 },
                        wgpu::VertexAttribute { offset: 24, shader_location: 2, format: wgpu::VertexFormat::Float32x3 },
                        wgpu::VertexAttribute { offset: 36, shader_location: 3, format: wgpu::VertexFormat::Float32x2 },
                        wgpu::VertexAttribute { offset: 44, shader_location: 4, format: wgpu::VertexFormat::Float32x4 },
                    ],
                }],
                compilation_options: wgpu::PipelineCompilationOptions::default(),
            },
            fragment: Some(wgpu::FragmentState {
                module: &shader,
                entry_point: "fs_main",
                targets: &[Some(wgpu::ColorTargetState {
                    format: wgpu::TextureFormat::Rgba8UnormSrgb,
                    blend: Some(wgpu::BlendState::ALPHA_BLENDING),
                    write_mask: wgpu::ColorWrites::ALL,
                })],
                compilation_options: wgpu::PipelineCompilationOptions::default(),
            }),
            primitive: wgpu::PrimitiveState {
                topology: wgpu::PrimitiveTopology::TriangleList,
                cull_mode: None,
                polygon_mode: wgpu::PolygonMode::Fill,
                ..Default::default()
            },
            depth_stencil: Some(wgpu::DepthStencilState {
                format: wgpu::TextureFormat::Depth32Float,
                depth_write_enabled: false, // Disable depth write for translucency
                depth_compare: wgpu::CompareFunction::Less,
                stencil: wgpu::StencilState::default(),
                bias: wgpu::DepthBiasState::default(),
            }),
            multisample: wgpu::MultisampleState::default(),
            multiview: None,
            cache: None,
        });

        let line_pipeline = device.create_render_pipeline(&wgpu::RenderPipelineDescriptor {
            label: Some("Line Pipeline"),
            layout: Some(&render_pipeline_layout),
            vertex: wgpu::VertexState {
                module: &shader,
                entry_point: "vs_main",
                buffers: &[wgpu::VertexBufferLayout {
                    array_stride: std::mem::size_of::<Vertex>() as wgpu::BufferAddress,
                    step_mode: wgpu::VertexStepMode::Vertex,
                    attributes: &[
                        wgpu::VertexAttribute { offset: 0, shader_location: 0, format: wgpu::VertexFormat::Float32x3 },
                        wgpu::VertexAttribute { offset: 12, shader_location: 1, format: wgpu::VertexFormat::Float32x3 },
                        wgpu::VertexAttribute { offset: 24, shader_location: 2, format: wgpu::VertexFormat::Float32x3 },
                        wgpu::VertexAttribute { offset: 36, shader_location: 3, format: wgpu::VertexFormat::Float32x2 },
                        wgpu::VertexAttribute { offset: 44, shader_location: 4, format: wgpu::VertexFormat::Float32x4 },
                    ],
                }],
                compilation_options: wgpu::PipelineCompilationOptions::default(),
            },
            fragment: Some(wgpu::FragmentState {
                module: &shader,
                entry_point: "fs_main",
                targets: &[Some(wgpu::ColorTargetState {
                    format: wgpu::TextureFormat::Rgba8UnormSrgb,
                    blend: Some(wgpu::BlendState::ALPHA_BLENDING),
                    write_mask: wgpu::ColorWrites::ALL,
                })],
                compilation_options: wgpu::PipelineCompilationOptions::default(),
            }),
            primitive: wgpu::PrimitiveState {
                topology: wgpu::PrimitiveTopology::LineList,
                ..Default::default()
            },
            depth_stencil: Some(wgpu::DepthStencilState {
                format: wgpu::TextureFormat::Depth32Float,
                depth_write_enabled: true,
                depth_compare: wgpu::CompareFunction::Less,
                stencil: wgpu::StencilState::default(),
                bias: wgpu::DepthBiasState::default(),
            }),
            multisample: wgpu::MultisampleState::default(),
            multiview: None,
            cache: None,
        });

        // --- Blit Pipeline ---
        let sampler = device.create_sampler(&wgpu::SamplerDescriptor {
            mag_filter: wgpu::FilterMode::Linear,
            min_filter: wgpu::FilterMode::Linear,
            ..Default::default()
        });
        
        let blit_bind_group_layout = device.create_bind_group_layout(&wgpu::BindGroupLayoutDescriptor {
            entries: &[
                wgpu::BindGroupLayoutEntry {
                    binding: 0,
                    visibility: wgpu::ShaderStages::FRAGMENT,
                    ty: wgpu::BindingType::Texture {
                        multisampled: false,
                        view_dimension: wgpu::TextureViewDimension::D2,
                        sample_type: wgpu::TextureSampleType::Float { filterable: true },
                    },
                    count: None,
                },
                wgpu::BindGroupLayoutEntry {
                    binding: 1,
                    visibility: wgpu::ShaderStages::FRAGMENT,
                    ty: wgpu::BindingType::Sampler(wgpu::SamplerBindingType::Filtering),
                    count: None,
                },
            ],
            label: Some("blit_bind_group_layout"),
        });

        let blit_pipeline_layout = device.create_pipeline_layout(&wgpu::PipelineLayoutDescriptor {
            label: Some("Blit Pipeline Layout"),
            bind_group_layouts: &[&blit_bind_group_layout],
            push_constant_ranges: &[],
        });

        let blit_pipeline = device.create_render_pipeline(&wgpu::RenderPipelineDescriptor {
            label: Some("Blit Pipeline"),
            layout: Some(&blit_pipeline_layout),
            vertex: wgpu::VertexState {
                module: &blit_shader,
                entry_point: "vs_main",
                buffers: &[],
                compilation_options: wgpu::PipelineCompilationOptions::default(),
            },
            fragment: Some(wgpu::FragmentState {
                module: &blit_shader,
                entry_point: "fs_main",
                targets: &[Some(wgpu::ColorTargetState {
                    format, // Output to screen format
                    blend: Some(wgpu::BlendState::REPLACE),
                    write_mask: wgpu::ColorWrites::ALL,
                })],
                compilation_options: wgpu::PipelineCompilationOptions::default(),
            }),
            primitive: wgpu::PrimitiveState::default(),
            depth_stencil: None, // No depth test for blit
            multisample: wgpu::MultisampleState::default(),
            multiview: None,
            cache: None,
        });

        // --- Geometry ---
        let (grid_verts, axis_verts) = create_grid_and_axis();
        let grid_buffer = device.create_buffer_init(&wgpu::util::BufferInitDescriptor {
            label: Some("Grid Buffer"),
            contents: bytemuck::cast_slice(&grid_verts),
            usage: wgpu::BufferUsages::VERTEX,
        });
        let axis_buffer = device.create_buffer_init(&wgpu::util::BufferInitDescriptor {
            label: Some("Axis Buffer"),
            contents: bytemuck::cast_slice(&axis_verts),
            usage: wgpu::BufferUsages::VERTEX,
        });

        Self {
            pbr_pipeline,
            shaded_pipeline,
            wireframe_pipeline,
            selection_pipeline,
            line_pipeline,
            translucent_pipeline,
            blit_pipeline,
            render_texture: None,
            render_view: None,
            depth_texture: None,
            depth_view: None,
            sampler,
            blit_bind_group: None,
            blit_bind_group_layout,
            camera,
            camera_controller,
            camera_uniform,
            camera_buffer,
            camera_bind_group,
            texture_bind_group_layout,
            default_texture_bind_group,
            texture_cache: std::collections::HashMap::new(),
            white_texture,
            grid_buffer,
            grid_count: grid_verts.len() as u32,
            axis_buffer,
            axis_count: axis_verts.len() as u32,
            meshes: Vec::new(),
            width: 0,
            height: 0,
            target_width: 0,
            target_height: 0,
            show_grid: true,
            show_axis: true,
            background_color: [0.9, 0.92, 0.95],
            display_mode: DisplayMode::Shaded,
            selected_mesh: None,
        }
    }

    pub fn set_selected_mesh(&mut self, selected: Option<usize>) {
        self.selected_mesh = selected;
    }

    pub fn add_mesh(&mut self, device: &wgpu::Device, queue: &wgpu::Queue, vertices: &[Vertex], indices: &[u32]) {
        self.add_mesh_with_label(device, queue, vertices, indices, None, None);
    }

    pub fn add_mesh_with_label(
        &mut self,
        device: &wgpu::Device,
        queue: &wgpu::Queue,
        vertices: &[Vertex],
        indices: &[u32],
        label: Option<String>,
        texture_path: Option<String>,
    ) {
        // ... (existing buffer creation)

        // Load texture if present and not already cached
        if let Some(path) = &texture_path {
            if !self.texture_cache.contains_key(path) {
                if let Ok(img) = image::open(path) {
                    let rgba = img.to_rgba8();
                    let dimensions = rgba.dimensions();

                    let texture_size = wgpu::Extent3d {
                        width: dimensions.0,
                        height: dimensions.1,
                        depth_or_array_layers: 1,
                    };

                    let texture = device.create_texture_with_data(
                        queue,
                        &wgpu::TextureDescriptor {
                            size: texture_size,
                            mip_level_count: 1,
                            sample_count: 1,
                            dimension: wgpu::TextureDimension::D2,
                            format: wgpu::TextureFormat::Rgba8UnormSrgb,
                            usage: wgpu::TextureUsages::TEXTURE_BINDING | wgpu::TextureUsages::COPY_DST,
                            label: Some(path),
                            view_formats: &[],
                        },
                        wgpu::util::TextureDataOrder::LayerMajor,
                        &rgba,
                    );

                    let view = texture.create_view(&wgpu::TextureViewDescriptor::default());
                    let sampler = device.create_sampler(&wgpu::SamplerDescriptor {
                        address_mode_u: wgpu::AddressMode::Repeat,
                        address_mode_v: wgpu::AddressMode::Repeat,
                        mag_filter: wgpu::FilterMode::Linear,
                        min_filter: wgpu::FilterMode::Linear,
                        mipmap_filter: wgpu::FilterMode::Nearest,
                        ..Default::default()
                    });

                    let bind_group = device.create_bind_group(&wgpu::BindGroupDescriptor {
                        layout: &self.texture_bind_group_layout,
                        entries: &[
                            wgpu::BindGroupEntry {
                                binding: 0,
                                resource: wgpu::BindingResource::TextureView(&view),
                            },
                            wgpu::BindGroupEntry {
                                binding: 1,
                                resource: wgpu::BindingResource::Sampler(&sampler),
                            },
                        ],
                        label: Some(path),
                    });

                    self.texture_cache.insert(path.clone(), bind_group);
                    println!("Loaded texture: {}", path);
                } else {
                    println!("Failed to load texture: {}", path);
                }
            }
        }
        let vertex_buffer = device.create_buffer_init(&wgpu::util::BufferInitDescriptor {
            label: Some("Mesh Vertex Buffer"),
            contents: bytemuck::cast_slice(vertices),
            usage: wgpu::BufferUsages::VERTEX,
        });

        let index_buffer = device.create_buffer_init(&wgpu::util::BufferInitDescriptor {
            label: Some("Mesh Index Buffer"),
            contents: bytemuck::cast_slice(indices),
            usage: wgpu::BufferUsages::INDEX,
        });

        // Calculate AABB
        let mut min = glam::Vec3::splat(f32::MAX);
        let mut max = glam::Vec3::splat(f32::MIN);
        let mut is_translucent = false;
        
        for v in vertices {
            let p = glam::Vec3::new(v.position[0], v.position[1], v.position[2]);
            min = min.min(p);
            max = max.max(p);
            if v.material[2] < 0.999 {
                is_translucent = true;
            }
        }
        
        if vertices.is_empty() {
            min = glam::Vec3::ZERO;
            max = glam::Vec3::ZERO;
        }
        let center = (min + max) * 0.5;

        self.meshes.push(RenderMesh {
            vertex_buffer,
            index_buffer,
            index_count: indices.len() as u32,
            aabb: (min, max),
            center,
            is_translucent,
            label,
            texture_path,
            pick_positions: vertices
                .iter()
                .map(|v| glam::Vec3::new(v.position[0], v.position[1], v.position[2]))
                .collect(),
            pick_indices: indices.to_vec(),
        });
    }    

    /// Pick a mesh by screen coordinates (pixels, origin at top-left of the viewport).
    /// Returns the index into `self.meshes` of the closest hit.
    pub fn pick_mesh(&self, x: f32, y: f32) -> Option<usize> {
        if self.width == 0 || self.height == 0 {
            return None;
        }

        let vp = self.camera.build_view_projection_matrix();
        let inv_vp = vp.inverse();

        // glam::Mat4::perspective_rh uses a 0..1 depth range (wgpu/Vulkan style).
        // Screen Y is down, so we flip it for NDC.
        let ndc_x = (x / self.width as f32) * 2.0 - 1.0;
        let ndc_y = 1.0 - (y / self.height as f32) * 2.0;

        let near_clip = glam::Vec4::new(ndc_x, ndc_y, 0.0, 1.0);
        let far_clip = glam::Vec4::new(ndc_x, ndc_y, 1.0, 1.0);

        let near_world = inv_vp * near_clip;
        let far_world = inv_vp * far_clip;
        if near_world.w.abs() < 1e-6 || far_world.w.abs() < 1e-6 {
            return None;
        }

        let origin = near_world.truncate() / near_world.w;
        let far_p = far_world.truncate() / far_world.w;
        let dir = (far_p - origin).normalize_or_zero();
        if dir.length_squared() < 1e-8 {
            return None;
        }

        fn ray_aabb(origin: glam::Vec3, dir: glam::Vec3, min: glam::Vec3, max: glam::Vec3) -> Option<f32> {
            // Slab method; returns entry distance t if hit in front of origin.
            let inv = glam::Vec3::new(
                if dir.x.abs() < 1e-8 { f32::INFINITY } else { 1.0 / dir.x },
                if dir.y.abs() < 1e-8 { f32::INFINITY } else { 1.0 / dir.y },
                if dir.z.abs() < 1e-8 { f32::INFINITY } else { 1.0 / dir.z },
            );
            let mut t1 = (min.x - origin.x) * inv.x;
            let mut t2 = (max.x - origin.x) * inv.x;
            let mut tmin = t1.min(t2);
            let mut tmax = t1.max(t2);

            t1 = (min.y - origin.y) * inv.y;
            t2 = (max.y - origin.y) * inv.y;
            tmin = tmin.max(t1.min(t2));
            tmax = tmax.min(t1.max(t2));

            t1 = (min.z - origin.z) * inv.z;
            t2 = (max.z - origin.z) * inv.z;
            tmin = tmin.max(t1.min(t2));
            tmax = tmax.min(t1.max(t2));

            if tmax >= tmin && tmax >= 0.0 {
                Some(if tmin >= 0.0 { tmin } else { tmax })
            } else {
                None
            }
        }

        fn ray_triangle(origin: glam::Vec3, dir: glam::Vec3, v0: glam::Vec3, v1: glam::Vec3, v2: glam::Vec3) -> Option<f32> {
            // Möller–Trumbore intersection, double-sided.
            let eps = 1e-7;
            let e1 = v1 - v0;
            let e2 = v2 - v0;
            let p = dir.cross(e2);
            let det = e1.dot(p);
            if det.abs() < eps {
                return None;
            }
            let inv_det = 1.0 / det;
            let tvec = origin - v0;
            let u = tvec.dot(p) * inv_det;
            if u < 0.0 || u > 1.0 {
                return None;
            }
            let q = tvec.cross(e1);
            let v = dir.dot(q) * inv_det;
            if v < 0.0 || (u + v) > 1.0 {
                return None;
            }
            let t = e2.dot(q) * inv_det;
            if t >= 0.0 { Some(t) } else { None }
        }

        let mut best: Option<(usize, f32)> = None;
        for (i, mesh) in self.meshes.iter().enumerate() {
            // Coarse reject by AABB, then precise triangle test.
            if ray_aabb(origin, dir, mesh.aabb.0, mesh.aabb.1).is_none() {
                continue;
            }

            let positions = &mesh.pick_positions;
            let inds = &mesh.pick_indices;
            if positions.is_empty() || inds.len() < 3 {
                continue;
            }

            let mut best_t_mesh: Option<f32> = None;
            let tri_count = inds.len() / 3;
            for ti in 0..tri_count {
                let i0 = inds[ti * 3] as usize;
                let i1 = inds[ti * 3 + 1] as usize;
                let i2 = inds[ti * 3 + 2] as usize;
                if i0 >= positions.len() || i1 >= positions.len() || i2 >= positions.len() {
                    continue;
                }
                if let Some(t) = ray_triangle(origin, dir, positions[i0], positions[i1], positions[i2]) {
                    best_t_mesh = match best_t_mesh {
                        None => Some(t),
                        Some(bt) if t < bt => Some(t),
                        Some(bt) => Some(bt),
                    };
                }
            }

            if let Some(t) = best_t_mesh {
                // Prefer nearer hits; if nearly tied, prefer translucent meshes (helps selecting glass over big walls).
                const T_EPS: f32 = 1e-4;
                match best {
                    None => best = Some((i, t)),
                    Some((best_i, best_t)) => {
                        if t + T_EPS < best_t {
                            best = Some((i, t));
                        } else if (t - best_t).abs() <= T_EPS {
                            let best_trans = self.meshes[best_i].is_translucent;
                            if mesh.is_translucent && !best_trans {
                                best = Some((i, t));
                            }
                        }
                    }
                }
            }
        }

        best.map(|(i, _)| i)
    }
    pub fn set_size(&mut self, width: u32, height: u32) {
        if width > 0 && height > 0 {
            self.target_width = width;
            self.target_height = height;
        }
    }

    fn resize(&mut self, device: &wgpu::Device, width: u32, height: u32) {
        if width > 0 && height > 0 {
            self.width = width;
            self.height = height;
            self.camera.aspect = width as f32 / height as f32;
            
            // Recreate textures
            let render_texture = device.create_texture(&wgpu::TextureDescriptor {
                label: Some("Render Texture"),
                size: wgpu::Extent3d { width, height, depth_or_array_layers: 1 },
                mip_level_count: 1,
                sample_count: 1,
                dimension: wgpu::TextureDimension::D2,
                format: wgpu::TextureFormat::Rgba8UnormSrgb,
                usage: wgpu::TextureUsages::RENDER_ATTACHMENT | wgpu::TextureUsages::TEXTURE_BINDING,
                view_formats: &[],
            });
            let render_view = render_texture.create_view(&wgpu::TextureViewDescriptor::default());

            let depth_texture = device.create_texture(&wgpu::TextureDescriptor {
                label: Some("Depth Texture"),
                size: wgpu::Extent3d { width, height, depth_or_array_layers: 1 },
                mip_level_count: 1,
                sample_count: 1,
                dimension: wgpu::TextureDimension::D2,
                format: wgpu::TextureFormat::Depth32Float,
                usage: wgpu::TextureUsages::RENDER_ATTACHMENT,
                view_formats: &[],
            });
            let depth_view = depth_texture.create_view(&wgpu::TextureViewDescriptor::default());

            // Update Blit BindGroup
            let blit_bind_group = device.create_bind_group(&wgpu::BindGroupDescriptor {
                layout: &self.blit_bind_group_layout,
                entries: &[
                    wgpu::BindGroupEntry {
                        binding: 0,
                        resource: wgpu::BindingResource::TextureView(&render_view),
                    },
                    wgpu::BindGroupEntry {
                        binding: 1,
                        resource: wgpu::BindingResource::Sampler(&self.sampler),
                    },
                ],
                label: Some("Blit Bind Group"),
            });

            self.render_texture = Some(render_texture);
            self.render_view = Some(render_view);
            self.depth_texture = Some(depth_texture);
            self.depth_view = Some(depth_view);
            self.blit_bind_group = Some(blit_bind_group);
        }
    }

    pub fn prepare(&mut self, device: &wgpu::Device, queue: &wgpu::Queue) -> Vec<wgpu::CommandBuffer> {
        // Handle resizing here
        if self.target_width > 0 && self.target_height > 0 {
            if self.target_width != self.width || self.target_height != self.height {
                 self.resize(device, self.target_width, self.target_height);
            }
        }
        
        if self.render_view.is_none() {
            return vec![];
        }

        // Update Camera
        self.camera_controller.update_camera(&mut self.camera);
        self.camera_uniform.update_view_proj(&self.camera);
        
        // Update Params based on display mode
        let mut alpha = 1.0;
        let mut style = 0.0;

        match self.display_mode {
            DisplayMode::Translucent => alpha = 0.4,
            DisplayMode::XRay => alpha = 0.3,
            DisplayMode::Artistic => style = 1.0,
            _ => {},
        }
        self.camera_uniform.params = [alpha, style, 0.0, 0.0];

        queue.write_buffer(&self.camera_buffer, 0, bytemuck::cast_slice(&[self.camera_uniform]));

        let mut encoder = device.create_command_encoder(&wgpu::CommandEncoderDescriptor {
            label: Some("3D Render Encoder"),
        });

        {
            let mut render_pass = encoder.begin_render_pass(&wgpu::RenderPassDescriptor {
                label: Some("3D Render Pass"),
                color_attachments: &[Some(wgpu::RenderPassColorAttachment {
                    view: self.render_view.as_ref().unwrap(),
                    resolve_target: None,
                    ops: wgpu::Operations {
                        load: wgpu::LoadOp::Clear(wgpu::Color { 
                            r: self.background_color[0] as f64, 
                            g: self.background_color[1] as f64, 
                            b: self.background_color[2] as f64, 
                            a: 1.0 
                        }),
                        store: wgpu::StoreOp::Store,
                    },
                })],
                depth_stencil_attachment: Some(wgpu::RenderPassDepthStencilAttachment {
                    view: self.depth_view.as_ref().unwrap(),
                    depth_ops: Some(wgpu::Operations {
                        load: wgpu::LoadOp::Clear(1.0),
                        store: wgpu::StoreOp::Store,
                    }),
                    stencil_ops: None,
                }),
                timestamp_writes: None,
                occlusion_query_set: None,
            });

            render_pass.set_bind_group(0, &self.camera_bind_group, &[]);

            // Draw Grid & Axes
            render_pass.set_pipeline(&self.line_pipeline);
            // Lines don't need textures, use default
            render_pass.set_bind_group(1, &self.default_texture_bind_group, &[]);
            
            if self.show_grid {
                render_pass.set_vertex_buffer(0, self.grid_buffer.slice(..));
                render_pass.draw(0..self.grid_count, 0..1);
            }
            
            if self.show_axis {
                render_pass.set_vertex_buffer(0, self.axis_buffer.slice(..));
                render_pass.draw(0..self.axis_count, 0..1);
            }

            // Draw Meshes
            match self.display_mode {
                DisplayMode::Wireframe => {
                    render_pass.set_pipeline(&self.wireframe_pipeline);
                    // Wireframe doesn't need textures, use default
                    render_pass.set_bind_group(1, &self.default_texture_bind_group, &[]);
                    for mesh in &self.meshes {
                        render_pass.set_vertex_buffer(0, mesh.vertex_buffer.slice(..));
                        render_pass.set_index_buffer(mesh.index_buffer.slice(..), wgpu::IndexFormat::Uint32);
                        render_pass.draw_indexed(0..mesh.index_count, 0, 0..1);
                    }
                },
                DisplayMode::Shaded | DisplayMode::MaterialPreview => {
                    // 1) Opaque pass (depth write ON)
                    match self.display_mode {
                        DisplayMode::Shaded => render_pass.set_pipeline(&self.shaded_pipeline),
                        DisplayMode::MaterialPreview => render_pass.set_pipeline(&self.pbr_pipeline),
                        _ => {}
                    }

                    for mesh in self.meshes.iter().filter(|m| !m.is_translucent) {
                        // Bind texture if present
                        if let Some(path) = &mesh.texture_path {
                            if let Some(bg) = self.texture_cache.get(path) {
                                render_pass.set_bind_group(1, bg, &[]);
                            } else {
                                render_pass.set_bind_group(1, &self.default_texture_bind_group, &[]);
                            }
                        } else {
                            render_pass.set_bind_group(1, &self.default_texture_bind_group, &[]);
                        }

                        render_pass.set_vertex_buffer(0, mesh.vertex_buffer.slice(..));
                        render_pass.set_index_buffer(mesh.index_buffer.slice(..), wgpu::IndexFormat::Uint32);
                        render_pass.draw_indexed(0..mesh.index_count, 0, 0..1);
                    }

                    // 2) Translucent pass (depth write OFF), sorted back-to-front
                    let cam_pos = self.camera.eye;
                    let mut translucent: Vec<&RenderMesh> = self.meshes.iter().filter(|m| m.is_translucent).collect();
                    translucent.sort_by(|a, b| {
                        let da = (a.center - cam_pos).length_squared();
                        let db = (b.center - cam_pos).length_squared();
                        db.partial_cmp(&da).unwrap_or(std::cmp::Ordering::Equal)
                    });

                    if !translucent.is_empty() {
                        render_pass.set_pipeline(&self.translucent_pipeline);
                        for mesh in translucent {
                            // Bind texture if present
                            if let Some(path) = &mesh.texture_path {
                                if let Some(bg) = self.texture_cache.get(path) {
                                    render_pass.set_bind_group(1, bg, &[]);
                                } else {
                                    render_pass.set_bind_group(1, &self.default_texture_bind_group, &[]);
                                }
                            } else {
                                render_pass.set_bind_group(1, &self.default_texture_bind_group, &[]);
                            }

                            render_pass.set_vertex_buffer(0, mesh.vertex_buffer.slice(..));
                            render_pass.set_index_buffer(mesh.index_buffer.slice(..), wgpu::IndexFormat::Uint32);
                            render_pass.draw_indexed(0..mesh.index_count, 0, 0..1);
                        }
                    }
                },
                DisplayMode::XRay => {
                     // 1. Draw Translucent Mesh
                     render_pass.set_pipeline(&self.translucent_pipeline);
                     for mesh in &self.meshes {
                        // Bind texture if present
                        if let Some(path) = &mesh.texture_path {
                            if let Some(bg) = self.texture_cache.get(path) {
                                render_pass.set_bind_group(1, bg, &[]);
                            } else {
                                render_pass.set_bind_group(1, &self.default_texture_bind_group, &[]);
                            }
                        } else {
                            render_pass.set_bind_group(1, &self.default_texture_bind_group, &[]);
                        }

                        render_pass.set_vertex_buffer(0, mesh.vertex_buffer.slice(..));
                        render_pass.set_index_buffer(mesh.index_buffer.slice(..), wgpu::IndexFormat::Uint32);
                        render_pass.draw_indexed(0..mesh.index_count, 0, 0..1);
                    }
                    
                    // 2. Draw Wireframe Overlay
                    render_pass.set_pipeline(&self.wireframe_pipeline);
                    // Wireframe doesn't need textures
                    render_pass.set_bind_group(1, &self.default_texture_bind_group, &[]);
                    for mesh in &self.meshes {
                        render_pass.set_vertex_buffer(0, mesh.vertex_buffer.slice(..));
                        render_pass.set_index_buffer(mesh.index_buffer.slice(..), wgpu::IndexFormat::Uint32);
                        render_pass.draw_indexed(0..mesh.index_count, 0, 0..1);
                    }
                },
                DisplayMode::Translucent => {
                     render_pass.set_pipeline(&self.translucent_pipeline);
                     for mesh in &self.meshes {
                        // Bind texture if present
                        if let Some(path) = &mesh.texture_path {
                            if let Some(bg) = self.texture_cache.get(path) {
                                render_pass.set_bind_group(1, bg, &[]);
                            } else {
                                render_pass.set_bind_group(1, &self.default_texture_bind_group, &[]);
                            }
                        } else {
                            render_pass.set_bind_group(1, &self.default_texture_bind_group, &[]);
                        }

                        render_pass.set_vertex_buffer(0, mesh.vertex_buffer.slice(..));
                        render_pass.set_index_buffer(mesh.index_buffer.slice(..), wgpu::IndexFormat::Uint32);
                        render_pass.draw_indexed(0..mesh.index_count, 0, 0..1);
                    }
                },
                DisplayMode::Artistic => {
                     // 1. Draw Shaded (Grayscale via shader)
                     render_pass.set_pipeline(&self.pbr_pipeline);
                     for mesh in &self.meshes {
                        // Bind texture if present
                        if let Some(path) = &mesh.texture_path {
                            if let Some(bg) = self.texture_cache.get(path) {
                                render_pass.set_bind_group(1, bg, &[]);
                            } else {
                                render_pass.set_bind_group(1, &self.default_texture_bind_group, &[]);
                            }
                        } else {
                            render_pass.set_bind_group(1, &self.default_texture_bind_group, &[]);
                        }

                        render_pass.set_vertex_buffer(0, mesh.vertex_buffer.slice(..));
                        render_pass.set_index_buffer(mesh.index_buffer.slice(..), wgpu::IndexFormat::Uint32);
                        render_pass.draw_indexed(0..mesh.index_count, 0, 0..1);
                    }
                    
                    // 2. Draw Wireframe Overlay
                    render_pass.set_pipeline(&self.wireframe_pipeline);
                    // Wireframe doesn't need textures
                    render_pass.set_bind_group(1, &self.default_texture_bind_group, &[]);
                     for mesh in &self.meshes {
                        render_pass.set_vertex_buffer(0, mesh.vertex_buffer.slice(..));
                        render_pass.set_index_buffer(mesh.index_buffer.slice(..), wgpu::IndexFormat::Uint32);
                        render_pass.draw_indexed(0..mesh.index_count, 0, 0..1);
                    }
                }
            }

            // Draw selection overlay (after main pass)
            if let Some(sel) = self.selected_mesh {
                if sel < self.meshes.len() {
                    let mesh = &self.meshes[sel];
                    render_pass.set_pipeline(&self.selection_pipeline);
                    render_pass.set_vertex_buffer(0, mesh.vertex_buffer.slice(..));
                    render_pass.set_index_buffer(mesh.index_buffer.slice(..), wgpu::IndexFormat::Uint32);
                    render_pass.draw_indexed(0..mesh.index_count, 0, 0..1);
                }
            }
        }

        vec![encoder.finish()]
    }

    pub fn paint(&self, render_pass: &mut wgpu::RenderPass<'static>) {
        if let Some(bg) = &self.blit_bind_group {
            render_pass.set_pipeline(&self.blit_pipeline);
            render_pass.set_bind_group(0, bg, &[]);
            render_pass.draw(0..3, 0..1); // Draw fullscreen triangle
        }
    }
}

fn create_grid_and_axis() -> (Vec<Vertex>, Vec<Vertex>) {
    let mut grid = Vec::new();
    let size = 20;
    let step = 1.0;
    let color = [0.4, 0.4, 0.4];
    let material = [0.0, 1.0, 1.0, 0.0]; // Non-metallic, rough, opaque, no emission
    
    // Grid Lines on XY plane (Z=0)
    for i in -size..=size {
        let val = i as f32 * step;
        let limit = size as f32 * step;
        
        // Parallel to X
        grid.push(Vertex { position: [-limit, val, 0.0], color, normal: [0.0, 0.0, 1.0], uv: [0.0, 0.0], material });
        grid.push(Vertex { position: [limit, val, 0.0], color, normal: [0.0, 0.0, 1.0], uv: [0.0, 0.0], material });
        
        // Parallel to Y
        grid.push(Vertex { position: [val, -limit, 0.0], color, normal: [0.0, 0.0, 1.0], uv: [0.0, 0.0], material });
        grid.push(Vertex { position: [val, limit, 0.0], color, normal: [0.0, 0.0, 1.0], uv: [0.0, 0.0], material });
    }

    let axis_material = [0.0, 0.6, 1.0, 0.0]; // Slightly glossy for axes
    let axis = vec![
        // X Axis (Red)
        Vertex { position: [0.0, 0.0, 0.0], color: [1.0, 0.0, 0.0], normal: [0.0, 0.0, 1.0], uv: [0.0, 0.0], material: axis_material },
        Vertex { position: [5.0, 0.0, 0.0], color: [1.0, 0.0, 0.0], normal: [0.0, 0.0, 1.0], uv: [0.0, 0.0], material: axis_material },
        // Y Axis (Green)
        Vertex { position: [0.0, 0.0, 0.0], color: [0.0, 1.0, 0.0], normal: [0.0, 0.0, 1.0], uv: [0.0, 0.0], material: axis_material },
        Vertex { position: [0.0, 5.0, 0.0], color: [0.0, 1.0, 0.0], normal: [0.0, 0.0, 1.0], uv: [0.0, 0.0], material: axis_material },
        // Z Axis (Blue)
        Vertex { position: [0.0, 0.0, 0.0], color: [0.0, 0.0, 1.0], normal: [0.0, 0.0, 1.0], uv: [0.0, 0.0], material: axis_material },
        Vertex { position: [0.0, 0.0, 5.0], color: [0.0, 0.0, 1.0], normal: [0.0, 0.0, 1.0], uv: [0.0, 0.0], material: axis_material },
    ];

    (grid, axis)
}