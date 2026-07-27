/*{
  "DESCRIPTION": "Diffuses the frame through frosted glass \u2014 a soft, privacy-glass scatter that keeps colour but dissolves detail.",
  "CATEGORIES": ["Guillotine", "Distortion"],
  "INPUTS": [
  {
    "NAME": "inputImage",
    "TYPE": "image"
  },
  {
    "NAME": "frost",
    "TYPE": "float",
    "DEFAULT": 0.5,
    "MIN": 0.0,
    "MAX": 1.0
  },
  {
    "NAME": "drift",
    "TYPE": "float",
    "DEFAULT": 0.25,
    "MIN": 0.0,
    "MAX": 1.0
  }
]
}*/
float hash(vec2 p) { return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453); }

void main() {
  vec2 uv = isf_FragNormCoord;
  vec2 t = 1.0 / RENDERSIZE;

  // Scatter each sample by a stable per-pixel offset, so the frost texture holds still and only
  // drifts when the user asks — random per frame would just look like noise.
  vec2 cell = floor(uv * RENDERSIZE / 3.0);
  float a = hash(cell) * 6.28318 + TIME * drift;
  float r = hash(cell + 7.3) * frost * 14.0;
  vec2 off = vec2(cos(a), sin(a)) * r * t;

  // Average a few scattered taps for a believable diffusion rather than a plain blur.
  vec3 col = IMG_NORM_PIXEL(inputImage, uv + off).rgb;
  col += IMG_NORM_PIXEL(inputImage, uv - off).rgb;
  col += IMG_NORM_PIXEL(inputImage, uv + vec2(-off.y, off.x)).rgb;
  col += IMG_NORM_PIXEL(inputImage, uv + vec2(off.y, -off.x)).rgb;
  col /= 4.0;

  vec4 c = IMG_THIS_PIXEL(inputImage);
  gl_FragColor = vec4(mix(c.rgb, col, frost), c.a);
}
