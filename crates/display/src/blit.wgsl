struct VertexOutput {
    @builtin(position) position: vec4<f32>,
    @location(0) uv: vec2<f32>,
};

@vertex
fn vs_main(@builtin(vertex_index) vertex_index: u32) -> VertexOutput {
    var out: VertexOutput;
    // Generates a triangle covering the screen: (0,0), (2,0), (0,2)
    // Clip space: (-1,-1), (3,-1), (-1,3)
    let uv = vec2<f32>(
        f32((vertex_index << 1u) & 2u),
        f32(vertex_index & 2u)
    );
    out.position = vec4<f32>(uv * 2.0 - 1.0, 0.0, 1.0);
    // Texture coordinates: (0, 1), (2, 1), (0, -1) ?
    // We want (0,0) top-left? wgpu coords: (0,0) top-left usually for texture?
    // standard UV: (0,0) top-left, (1,1) bottom-right in wgpu/metal? 
    // Wait, wgpu texture coords: (0,0) is usually top-left.
    out.uv = vec2<f32>(uv.x, 1.0 - uv.y); 
    return out;
}

@group(0) @binding(0) var t_diffuse: texture_2d<f32>;
@group(0) @binding(1) var s_diffuse: sampler;

@fragment
fn fs_main(in: VertexOutput) -> @location(0) vec4<f32> {
    return textureSample(t_diffuse, s_diffuse, in.uv);
}

