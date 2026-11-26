shader_type canvas_item;

uniform float blur = 0.0;
uniform vec3 color : source_color = vec3(1.0);
uniform sampler2D screen_texture : hint_screen_texture, repeat_disable;

void fragment() {
	// Called for every pixel the material is visible on.
	float alpha = texture(TEXTURE, UV).a;
	vec3 pixel_color = textureLod(screen_texture, SCREEN_UV, blur).rgb;
	COLOR = vec4(color * length(pixel_color), alpha);
}
