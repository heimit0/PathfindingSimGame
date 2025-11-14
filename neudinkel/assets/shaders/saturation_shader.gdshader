shader_type spatial;
render_mode blend_mix, cull_back, depth_draw_opaque;

// how intense the color is
uniform float saturation : hint_range(0.0, 2.0) = 1.0;
// optional: if your tile has a texture
uniform sampler2D albedo_texture : source_color;

void fragment() {
    // get the color, using the albedo texture if present
    vec3 color = texture(albedo_texture, UV).rgb;

    // compute grayscale luminance
    float gray = dot(color, vec3(0.299, 0.587, 0.114));

    // interpolate between gray and the original color
    color = mix(vec3(gray), color, saturation);

    ALBEDO = color;
}
