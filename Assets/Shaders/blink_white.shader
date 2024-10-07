shader_type canvas_item;

uniform float white_progress : hint_range(0,1) = 0;
uniform float white_progress2 : hint_range(0,1) = 0;
uniform vec4 flash_color: hint_color = vec4(1, 1, 1, 1);

vec3 interpolate_vec3(vec3 start, vec3 end, float delta){
    return start + (end - start) * delta;
}

void fragment(){
    vec4 origin = texture(TEXTURE, UV);
    COLOR.rgb = interpolate_vec3(origin.rgb, flash_color.rgb, max(white_progress, white_progress2));
    COLOR.a = origin.a;
}