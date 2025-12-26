struct CameraUniform {
    view_proj: mat4x4<f32>,
    params: vec4<f32>, // x: alpha, y: mode
    camera_pos: vec4<f32>,
};

@group(0) @binding(0)
var<uniform> camera: CameraUniform;

@group(1) @binding(0)
var t_diffuse: texture_2d<f32>;
@group(1) @binding(1)
var s_diffuse: sampler;

struct VertexInput {
    @location(0) position: vec3<f32>,
    @location(1) color: vec3<f32>,
    @location(2) normal: vec3<f32>,
    @location(3) uv: vec2<f32>,
    @location(4) material: vec4<f32>, // x: metallic, y: roughness, z: opacity, w: emission
};

struct VertexOutput {
    @builtin(position) clip_position: vec4<f32>,
    @location(0) color: vec3<f32>,
    @location(1) world_pos: vec3<f32>,
    @location(2) normal: vec3<f32>,
    @location(3) uv: vec2<f32>,
    @location(4) material: vec4<f32>,
};

@vertex
fn vs_main(
    model: VertexInput,
) -> VertexOutput {
    var out: VertexOutput;
    out.color = model.color;
    out.world_pos = model.position;
    out.normal = model.normal;
    out.uv = model.uv;
    out.material = model.material;
    out.clip_position = camera.view_proj * vec4<f32>(model.position, 1.0);
    return out;
}

// PBR Helper Functions
fn distribution_ggx(N: vec3<f32>, H: vec3<f32>, roughness: f32) -> f32 {
    let a = roughness * roughness;
    let a2 = a * a;
    let NdotH = max(dot(N, H), 0.0);
    let NdotH2 = NdotH * NdotH;
    
    let nom = a2;
    var denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = 3.14159265359 * denom * denom;
    
    return nom / max(denom, 0.0001);
}

fn geometry_schlick_ggx(NdotV: f32, roughness: f32) -> f32 {
    let r = (roughness + 1.0);
    let k = (r * r) / 8.0;
    
    let nom = NdotV;
    let denom = NdotV * (1.0 - k) + k;
    
    return nom / max(denom, 0.0001);
}

fn geometry_smith(N: vec3<f32>, V: vec3<f32>, L: vec3<f32>, roughness: f32) -> f32 {
    let NdotV = max(dot(N, V), 0.0);
    let NdotL = max(dot(N, L), 0.0);
    let ggx2 = geometry_schlick_ggx(NdotV, roughness);
    let ggx1 = geometry_schlick_ggx(NdotL, roughness);
    
    return ggx1 * ggx2;
}

fn fresnel_schlick(cosTheta: f32, F0: vec3<f32>) -> vec3<f32> {
    return F0 + (1.0 - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}

fn fresnel_schlick_roughness(cosTheta: f32, F0: vec3<f32>, roughness: f32) -> vec3<f32> {
    return F0 + (max(vec3<f32>(1.0 - roughness), F0) - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32> {
    // 1. Artistic Mode (Sketch)
    if (camera.params.y > 0.5 && camera.params.y < 1.5) {
        let dx = dpdx(in.world_pos);
        let dy = dpdy(in.world_pos);
        let N = normalize(cross(dx, dy));
        let V = normalize(camera.camera_pos.xyz - in.world_pos);
        let NdotV = abs(dot(N, V));
        let shade = pow(NdotV, 0.5); 
        let intensity = mix(0.7, 0.98, shade);
        return vec4<f32>(intensity, intensity, intensity, 1.0);
    }
    
    // ===== Material Properties =====
    let metallic = in.material.x;
    let roughness = clamp(in.material.y, 0.04, 1.0); // Clamp to avoid singularities
    let opacity = in.material.z;
    let emission_strength = in.material.w;
    
    // Base color with sRGB to linear conversion
    var albedo = pow(in.color, vec3<f32>(2.2));
    
    // Apply texture if alpha is not 0 (0 alpha means not transparent but missing texture in our default white texture)
    // But since we use a white texture [1,1,1,1] by default, we can just multiply.
    let tex_color = textureSample(t_diffuse, s_diffuse, in.uv);
    albedo = albedo * pow(tex_color.rgb, vec3<f32>(2.2));
    
    // Normal & View Dir
    let N = normalize(in.normal);
    let V = normalize(camera.camera_pos.xyz - in.world_pos);
    
    // Calculate reflectance at normal incidence
    // For dielectrics use 0.04, for metals use albedo color
    let F0 = mix(vec3<f32>(0.04), albedo, metallic);
    
    // ===== PBR Lighting =====
    var Lo = vec3<f32>(0.0);
    
    // Light 1: Key Light (Main directional light)
    {
        let L = normalize(vec3<f32>(-0.5, -1.0, 0.8));
        let H = normalize(V + L);
        let radiance = vec3<f32>(1.0, 0.98, 0.95) * 2.0;
        
        // Cook-Torrance BRDF
        let NDF = distribution_ggx(N, H, roughness);
        let G = geometry_smith(N, V, L, roughness);
        let F = fresnel_schlick(max(dot(H, V), 0.0), F0);
        
        let numerator = NDF * G * F;
        let NdotL = max(dot(N, L), 0.0);
        let NdotV = max(dot(N, V), 0.0);
        let denominator = 4.0 * NdotV * NdotL + 0.0001;
        let specular = numerator / denominator;
        
        // For energy conservation
        let kS = F;
        var kD = vec3<f32>(1.0) - kS;
        kD *= 1.0 - metallic;
        
        Lo += (kD * albedo / 3.14159265359 + specular) * radiance * NdotL;
    }
    
    // Light 2: Fill Light
    {
        let L = normalize(vec3<f32>(1.0, -0.5, -0.3));
        let H = normalize(V + L);
        let radiance = vec3<f32>(0.85, 0.88, 1.0) * 1.2;
        
        let NDF = distribution_ggx(N, H, roughness);
        let G = geometry_smith(N, V, L, roughness);
        let F = fresnel_schlick(max(dot(H, V), 0.0), F0);
        
        let numerator = NDF * G * F;
        let NdotL = max(dot(N, L), 0.0);
        let NdotV = max(dot(N, V), 0.0);
        let denominator = 4.0 * NdotV * NdotL + 0.0001;
        let specular = numerator / denominator;
        
        let kS = F;
        var kD = vec3<f32>(1.0) - kS;
        kD *= 1.0 - metallic;
        
        Lo += (kD * albedo / 3.14159265359 + specular) * radiance * NdotL;
    }
    
    // Light 3: Rim/Back Light
    {
        let L = normalize(vec3<f32>(0.0, 0.3, -1.0));
        let H = normalize(V + L);
        let radiance = vec3<f32>(0.9, 0.92, 1.0) * 0.8;
        
        let NDF = distribution_ggx(N, H, roughness);
        let G = geometry_smith(N, V, L, roughness);
        let F = fresnel_schlick(max(dot(H, V), 0.0), F0);
        
        let numerator = NDF * G * F;
        let NdotL = max(dot(N, L), 0.0);
        let NdotV = max(dot(N, V), 0.0);
        let denominator = 4.0 * NdotV * NdotL + 0.0001;
        let specular = numerator / denominator;
        
        let kS = F;
        var kD = vec3<f32>(1.0) - kS;
        kD *= 1.0 - metallic;
        
        Lo += (kD * albedo / 3.14159265359 + specular) * radiance * NdotL;
    }
    
    // ===== Ambient/IBL (Simplified) =====
    let F_ambient = fresnel_schlick_roughness(max(dot(N, V), 0.0), F0, roughness);
    let kS_ambient = F_ambient;
    var kD_ambient = 1.0 - kS_ambient;
    kD_ambient *= 1.0 - metallic;
    
    // Hemisphere lighting for ambient
    let up_factor = N.y * 0.5 + 0.5;
    let sky_color = vec3<f32>(0.6, 0.7, 0.9);
    let ground_color = vec3<f32>(0.4, 0.35, 0.3);
    let hemisphere = mix(ground_color, sky_color, up_factor);
    
    let irradiance = hemisphere * 0.6;
    let diffuse_ibl = irradiance * albedo;
    
    // Simple specular IBL approximation
    let prefilteredColor = hemisphere * (1.0 - roughness * 0.5);
    let specular_ibl = prefilteredColor * (F_ambient * (1.0 - roughness) + roughness * 0.1);
    
    let ambient = (kD_ambient * diffuse_ibl + specular_ibl) * 0.8;
    
    // ===== Final Color =====
    var color = ambient + Lo;
    
    // Add emission
    color += albedo * emission_strength * 2.0;
    
    // HDR tonemapping (ACES approximation)
    color = (color * (2.51 * color + 0.03)) / (color * (2.43 * color + 0.59) + 0.14);
    
    // Gamma correction (linear to sRGB)
    color = pow(color, vec3<f32>(1.0 / 2.2));
    
    // Clamp
    color = clamp(color, vec3<f32>(0.0), vec3<f32>(1.0));
    
    // Apply opacity
    let final_alpha = opacity * camera.params.x;

    return vec4<f32>(color, final_alpha);
}

// Simple "Shaded" mode (non-PBR) to make it visually distinct from Material Preview.
@fragment
fn fs_shaded(in: VertexOutput) -> @location(0) vec4<f32> {
    // Basic lambert + small specular, no metallic/roughness workflow
    let N = normalize(in.normal);
    let V = normalize(camera.camera_pos.xyz - in.world_pos);

    // One key directional light
    let L = normalize(vec3<f32>(-0.5, -1.0, 0.8));
    let H = normalize(V + L);

    let NdotL = max(dot(N, L), 0.0);
    let NdotH = max(dot(N, H), 0.0);

    // Base color (still treat vertex color as sRGB)
    var albedo = pow(in.color, vec3<f32>(2.2));
    let tex_color = textureSample(t_diffuse, s_diffuse, in.uv);
    albedo = albedo * pow(tex_color.rgb, vec3<f32>(2.2));

    // Ambient + diffuse
    var color = albedo * (0.25 + 0.9 * NdotL);

    // Small specular highlight
    color += vec3<f32>(0.04) * pow(NdotH, 32.0) * (0.2 + 0.8 * NdotL);

    // Gamma back to sRGB
    color = pow(clamp(color, vec3<f32>(0.0), vec3<f32>(1.0)), vec3<f32>(1.0 / 2.2));

    // Shaded mode always outputs opaque (viewport alpha is handled by Rust when needed)
    return vec4<f32>(color, 1.0);
}

@fragment
fn fs_wireframe(in: VertexOutput) -> @location(0) vec4<f32> {
    var final_color = in.color;
    
    if (camera.params.y > 0.5 && camera.params.y < 1.5) {
        final_color = vec3<f32>(0.0, 0.0, 0.0);
    }
    
    return vec4<f32>(final_color, 1.0);
}

// Selection highlight (constant color)
@fragment
fn fs_selection(_in: VertexOutput) -> @location(0) vec4<f32> {
    return vec4<f32>(1.0, 0.85, 0.2, 1.0);
}