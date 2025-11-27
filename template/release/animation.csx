var rect = Add(new Rectangle(100,100));
var rect2 = Add(new Rectangle(10,100));

rect.Move(100,100);
rect2.Move(1000,0);
Go(1);

rect.Move(100,200);
Go(1);