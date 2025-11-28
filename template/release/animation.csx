var rect = Add(new Rectangle(100,100));
var rect2 = Add(new Rectangle(10,100));

rect.Move(-10,100);
rect2.Move(10000,0);
Go(60 * 10);

rect.Move(100,200);
Go(10);