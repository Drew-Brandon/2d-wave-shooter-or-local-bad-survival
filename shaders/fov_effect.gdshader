shader_type canvas_item;
render_mode world_vertex_coords, unshaded;

varying vec2 px_pos;

uniform int fov_vert_count = 0;
uniform vec2 fov_verts[360];
uniform float blur = 0.0;
uniform vec4 color : source_color = vec4(1.0);
uniform sampler2D screen_texture : hint_screen_texture, repeat_disable;

// https://www.geeksforgeeks.org/dsa/how-to-check-if-a-given-point-lies-inside-a-polygon/
bool on_line(vec2 start, vec2 end, vec2 point) {
	float c = (point.x - start.x) * (end.y - start.y) - (point.y - start.y) * (end.x - start.x);
	
	if (c != 0.0) {
		return false;
	}
	
	return min(start.x, end.x) <= point.x && point.x <= max(start.x, end.x) &&
		min(start.y, end.y) <= point.y && point.y <= max(start.y, end.y);
}

bool on_level_with_line(vec2 start, vec2 end, vec2 point) {
	return ((start.y > point.y) != (end.y > point.y) &&
		point.x < (end.x - start.x) * (point.y - start.y) / (end.y - start.y) + start.x);
}

bool in_fov(vec2 point) {
	bool inside = false;
	int prev_i = fov_vert_count - 1;
	
	for (int i = 0; i < fov_vert_count; i++)
	{
		vec2 start = fov_verts[i];
		vec2 end = fov_verts[prev_i];
		
		if (on_line(start, end, point)) {
			return true;
		}
		
		if (on_level_with_line(start, end, point)) {
			inside = !inside;
		}
		
		prev_i = i;
	}
	
	return inside;
}

void vertex() {
	px_pos = VERTEX;
}

void fragment() {
	// Called for every pixel the material is visible on.
	vec4 pixel_color = textureLod(screen_texture, SCREEN_UV, blur);
	
	if (in_fov(px_pos)) {
		COLOR = vec4(0.0);
	} else {
		COLOR = vec4(color.rgb * length(pixel_color.rgb), color.a);
	}
}
