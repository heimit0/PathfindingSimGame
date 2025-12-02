shader_type canvas_item;

// Base grey value (0 = black, 1 = white)
uniform float base_grey : hint_range(0.0, 1.0) = 0.5;

// Fixed transparency (can now be adjusted)
uniform float alpha_value : hint_range(0.0, 1.0) = 0.6;

// How strong the diagonal gradient is (0 = no gradient, 1 = full gradient)
uniform float gradient_strength : hint_range(0.0, 1.0) = 0.05;

void fragment() {
    // Diagonal factor from top-left (0) to bottom-right (1)
    float t = (UV.x + UV.y) * 0.5;

    // Compute grey value along the diagonal
    float grey = base_grey + gradient_strength * (t - 0.5);

    COLOR = vec4(grey, grey, grey, alpha_value);
}
