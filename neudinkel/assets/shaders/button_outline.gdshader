shader_type canvas_item;

uniform vec4 outline_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float thickness = 15.0;

void fragment() {
    vec2 tex_size = vec2(textureSize(TEXTURE, 0));
    vec2 pixel = SCREEN_PIXEL_SIZE * thickness;

    float alpha = texture(TEXTURE, UV).a;

    if (alpha > 0.0) {
        // Normal visible pixel
        COLOR = texture(TEXTURE, UV);
    } else {
        // Check neighbors for outline
        float outline = 0.0;
        outline += texture(TEXTURE, UV + vec2(pixel.x, 0.0)).a;
        outline += texture(TEXTURE, UV + vec2(-pixel.x, 0.0)).a;
        outline += texture(TEXTURE, UV + vec2(0.0, pixel.y)).a;
        outline += texture(TEXTURE, UV + vec2(0.0, -pixel.y)).a;

        if (outline > 0.0) {
            COLOR = outline_color;
        } else {
            discard;
        }
    }
}
