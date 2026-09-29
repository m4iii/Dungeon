# Imagegen 地形候选

使用内置 image_gen 生成，参考原包 hexHighlands00 与 hexPlains00 的视角及绘制风格。原素材未修改，当前地图已试用本目录图片：森林/草原使用草地岩脊，沙漠使用砂岩；雪地和方格模式保留原素材。

- hex-grass-ridge-v1.png：草地低矮岩脊，1024×1536 RGBA。
- hex-sandstone-ridge-v1.png：沙漠分层砂岩，1024×1536 RGBA。

状态：已接入地图试用，尚非最终美术。渲染端按六边形地面轮廓裁剪外缘柔光，原始 PNG 保留。外缘仍有半透明柔光，尚未通过透明边缘、相邻拼接与缩小后的可读性验证。一次清理草地边缘的生成尝试未改善，未选用该版本。

## 草地提示词

Use case: precise-object-edit. Asset type: a transparent production candidate hex terrain sprite for a 2D fantasy strategy game. Image 1 is the EDIT TARGET; image 2 is a style/color reference only. Repaint image 1 into a LOW CONTINUOUS ROCKY RIDGE obstacle tile, matching the painterly illustrated strategy terrain of both references: compact brushwork, restrained crisp edges, muted olive grass, warm gray stone, soft upper-left light, gentle anime illustration influence without glossy cartoon outlines. Preserve the target's EXACT pointy-top hex ground footprint and viewing angle: canvas 2:3 portrait, ground hex vertices at normalized (0.5,0.333),(1,0.5),(1,0.833),(0.5,1),(0,0.833),(0,0.5); empty transparent upper third. Fill that footprint edge to edge. Replace the busy random pile of many little rocks with 3-4 broad connected stratified stone shelves forming a low natural escarpment, subtle grassy seams. Height only modestly above ground, no giant mountain, no isolated standing pillar, no central tower, no trench, no walls made of bricks. Ground and rocks should look like one coherent landscape fragment, able to repeat next to its own kind without a decorative perimeter. No raised hex rim, no extra border, no thick plinth, no shadow outside footprint, no text, no UI, no checkerboard or colored background. One single tile, not a sheet, actual transparent alpha. Keep stone and vegetation scale comparable to reference.

## 沙漠提示词

Use case: precise-object-edit. Make a DESERT SANDSTONE version of the provided hex terrain sprite. Keep its exact 2:3 portrait canvas, pointy-top hex ground silhouette, 2D oblique view, top-left illumination and traditional hand-painted fantasy strategy game art style. Replace every green grassy surface with muted pale ochre sand and every rock with warm buff layered sandstone. Simplify the terrain into three broad connected LOW stone shelves with a few cracks and sand pockets, not a chaotic heap. No foliage, no flowers, no water, no tall mountains or spires, no buildings. Preserve empty top third and the original full-width ground footprint in the bottom two thirds. Single tile only. The terrain should read as a continuous low impassable ridge beside pale sand dunes, with readable large shapes at 256px sprite scale. Actual transparent background: absolutely no haze, glow, drop shadow, vignette, checkerboard, or colored backdrop outside the hex. No added frame, beveled hex rim, plinth or text.

## 未采用的边缘清理提示词

Use case: precise-object-edit. Edit this supplied sprite ONLY to clean its cutout. Preserve the painted grassy rocky terrain, low stone shelves, colors, exact perspective and composition. Output a real transparent PNG. Remove ALL colored glow, halo, drop shadow and stray alpha outside the terrain silhouette; upper empty canvas must be completely transparent, not translucent black or green. Keep crisp but naturally antialiased grass silhouette at upper edge. Keep the same pointy-top hex footprint, same 2:3 portrait canvas and exact scale. No added backing, no frame, no vignette. Leave all interior terrain pixels visually unchanged. This must be a clean game sprite for adjacent hex tiling.

