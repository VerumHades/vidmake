var rect = Add(new Rectangle(100,100));
rect.BackgroundColor = Pixel.Red;
var rect2 = Add(new Rectangle(50,100));

rect.Move(100,0);
rect2.Move(100,0);
Go(1);
rect.zIndex.Value = 1;
rect2.Move(100,200);
rect.Move(100,200);
Go(1);

rect2.zIndex.Value = 2;
rect2.Move(300,200);
rect.Move(300,200);
Go(1);