

using AbstractRendering;
using RawRendering;

RawRenderTarget target = new(1920, 1080, 30);
Scene scene = new(target);

var rect = scene.Add(new Rectangle(100,100));
var rect2 = scene.Add(new Rectangle(10,100));

rect.Move(100,100);
rect2.Move(1000,0);
scene.Go(1);

rect.Move(100,200);
scene.Go(1);

target.SaveToVideo("video.mp4");