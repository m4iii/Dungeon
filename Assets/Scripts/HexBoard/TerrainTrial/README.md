# 地形组合试验

打开 `Assets/Scenes/HexTerrainTrial.unity`。

网格采用与作者相同的上下尖顶轮廓：宽高均为 2，行距 1.5，相邻行横向错开 1。底座、选中轮廓、碰撞体和侧面共用该几何定义；厚度沿屏幕向下延伸，Pivot 保持顶面中心。

地表完整映射作者原始草地六边形，保留原素材的边缘与透明度，按原图比例拼接。没有额外描边、暗缝或地表调色。

Adjustable 2D sides 使用样包的 hexUnderDirt00.png，保留其两侧明暗和土层纹理。Thickness 只改变侧面深度，纹理随深度伸缩，不改变顶面；为 0 时隐藏侧面。共用材质 HexSides 的 Painted side texture 可替换兼容布局的侧面图片。

- 左侧：19 个原有 BaseHexCell 实例，四种地形配置复用草地、树群和池塘。
- 右侧：7 个作者原版地块，展示草地、森林、山体，保留原素材比例。
- 原棋盘保存在 `Assets/Scenes/HexBoard2D.unity`。

选中左侧地块，在 Hex Terrain Recipe 中修改 Terrain / Seed，然后从组件菜单选择 Rebuild Terrain。更改 Hex Cell Thickness 的 Thickness 可独立调整厚度。

配置文件 01 Meadow、02 Woodland、03 Forest、04 Pond 保存材质、装饰 Sprite、数量与大小。树群使用底部落地点作为 Pivot，并通过独立根排序组按世界 Y 排序。地表继续使用原底座几何轮廓。未来角色应与装饰使用同一排序约定。

当前只验证模块化组合，未完成最终二次元美术。草地完整使用同一张原始地表，后续可以补充变化。免费样包缺少独立山体，所以山体暂时仅在右侧整块对照中展示。尚未实现道路、河流、岸线连接。

来源：https://dgbaumgart.itch.io/hex-and-tile-terrain-sample-set

作者：David Baumgart。原始授权与说明见 Art/LICENSE.txt。素材仅作为本项目资源使用，不作为独立素材包分发。

## 未探索区域

`Assets/Scenes/HexRevealTrial.unity` 是揭格试验。未探索板块使用连续浅羊皮纸和稀疏的跨格制图装饰；图案由着色器生成，没有使用《文明6》的游戏素材。纸面坐标由 axial 推导并附着于面板，翻转时不会滑动。原先每格重复的大罗盘已移除。厚板翻转的透视和投影仍待下一步改进。
