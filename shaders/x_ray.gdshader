shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, repeat_disable;

void fragment() {
	// Called for every pixel the material is visible on.
	COLOR = textureLod(screen_texture, SCREEN_UV, 0.0);
}
