# XNSubstitute

My custom NSubstitute extensions.

# Integration with assertion frameworks:

```csharp
//GIVEN
var xyz = Substitute.For<IXyz>();

//WHEN
xyz.Do(new List<int>() { 1,2,3 });
xyz.Do(new List<int>() { 6,5,4 });

//THEN
xyz.Received(1).Do(Arg<List<int>>.That(
  l => l.Should().BeInAscendingOrder(),
  l => l.Should().Contain(1),
  l => l.Should().Contain(2),
  l => l.Should().Contain(3)));
```

# ReceivedNothing() extension method

```csharp
var sub = Substitute.For<IEnumerable>();

sub.ReceivedNothing();
```

# XReceived.Only()

```csharp
var foo = Substitute.For<IFoo>();
var bar = Substitute.For<IBar>();

bar.End();
foo.Start(2);
foo.Finish();
bar.Begin();

XReceived.Only(() =>
{
  foo.Start(2);
  bar.Begin();
  foo.Finish();
  bar.End();
});
```
# XReceived.Exactly()

```csharp
var foo = Substitute.For<IFoo>();
var bar = Substitute.For<IBar>();

foo.Start(2);
bar.Begin();
foo.Finish();
bar.End();

XReceived.Only(() =>
{
  foo.Start(2);
  bar.Begin();
  foo.Finish();
  bar.End();
});
```



