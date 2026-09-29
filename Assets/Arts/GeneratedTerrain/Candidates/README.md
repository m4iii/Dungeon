# 当前试用：v2（2026-09-29）

使用内置 image_gen 编辑原包参考图，替换四个森林/草原主题的 wallHexTexture。草地 v1 保留供回退。两版生成沙地已按要求删除，两个沙漠主题恢复原包岩石墙。未改地图生成、河流开关或雪地配置。

- hex-grass-ridge-v2.png：参考 hexHighlands00，保留原构图与中等岩石尺度，减少白花点、碎石与细纹，采用较克制的灰绿/灰石色。
- 原始 1024×1536 RGBA 不修改；Unity 导入上限 512，双线性、无 mipmap、无压缩。继续使用现有六边形轮廓裁剪。
- 状态：图片已检查，文件及四个森林/草原主题引用已更新。Unity 未运行，MCP 8080 未连接，尚未验证导入与地图内拼接。仍是试用版，不视为已完全匹配原素材。

## v2 草地实际生成提示词

Edit the supplied small game tile with a LIGHT TOUCH. Preserve the original drawing almost exactly: same composition, perspective, hex silhouette, olive muted grass, gray rocks, brightness and painterly rendering. This is a replacement within an existing 256x384 tile set, so stylistic fidelity is more important than making a new illustration. Remove white flower specks and a few of the smallest pebble clusters, reducing fine detail by about one third. Keep the existing medium-size rocks at their original size and distribution, retain broad rough brush texture and soft painted shading. Do NOT enlarge rocks, flatten them into solid polygons, make smooth vector art, sharpen, enhance realism, or add detail. Preserve original subtle muted palette; no vivid lime green, no yellow-gold wash. Keep exact 2:3 portrait sprite canvas, empty upper third, the same pointy-top hex footprint in the lower two thirds. Clean transparent exterior, no glow, no rim, no cast shadow outside the ground. One tile only.


---

以下为 v1 历史记录，已不用于当前主题。

# Imagegen 地形候选

使用内置 image_gen 生成，参考原包 hexHighlands00 与 hexPlains00 的视角及绘制风格。原素材未修改，上一版地图曾试用本目录 v1 图片：森林/草原使用草地岩脊，沙漠使用砂岩；雪地和方格模式保留原素材。

- hex-grass-ridge-v1.png：草地低矮岩脊，1024×1536 RGBA。

状态：已接入地图试用，尚非最终美术。渲染端按六边形地面轮廓裁剪外缘柔光，原始 PNG 保留。外缘仍有半透明柔光，尚未通过透明边缘、相邻拼接与缩小后的可读性验证。一次清理草地边缘的生成尝试未改善，未选用该版本。

## 草地提示词

Use case: precise-object-edit. Asset type: a transparent production candidate hex terrain sprite for a 2D fantasy strategy game. Image 1 is the EDIT TARGET; image 2 is a style/color reference only. Repaint image 1 into a LOW CONTINUOUS ROCKY RIDGE obstacle tile, matching the painterly illustrated strategy terrain of both references: compact brushwork, restrained crisp edges, muted olive grass, warm gray stone, soft upper-left light, gentle anime illustration influence without glossy cartoon outlines. Preserve the target's EXACT pointy-top hex ground footprint and viewing angle: canvas 2:3 portrait, ground hex vertices at normalized (0.5,0.333),(1,0.5),(1,0.833),(0.5,1),(0,0.833),(0,0.5); empty transparent upper third. Fill that footprint edge to edge. Replace the busy random pile of many little rocks with 3-4 broad connected stratified stone shelves forming a low natural escarpment, subtle grassy seams. Height only modestly above ground, no giant mountain, no isolated standing pillar, no central tower, no trench, no walls made of bricks. Ground and rocks should look like one coherent landscape fragment, able to repeat next to its own kind without a decorative perimeter. No raised hex rim, no extra border, no thick plinth, no shadow outside footprint, no text, no UI, no checkerboard or colored background. One single tile, not a sheet, actual transparent alpha. Keep stone and vegetation scale comparable to reference.

## 未采用的边缘清理提示词

Use case: precise-object-edit. Edit this supplied sprite ONLY to clean its cutout. Preserve the painted grassy rocky terrain, low stone shelves, colors, exact perspective and composition. Output a real transparent PNG. Remove ALL colored glow, halo, drop shadow and stray alpha outside the terrain silhouette; upper empty canvas must be completely transparent, not translucent black or green. Keep crisp but naturally antialiased grass silhouette at upper edge. Keep the same pointy-top hex footprint, same 2:3 portrait canvas and exact scale. No added backing, no frame, no vignette. Leave all interior terrain pixels visually unchanged. This must be a clean game sprite for adjacent hex tiling.

