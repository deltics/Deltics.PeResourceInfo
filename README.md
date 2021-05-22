# Deltics.PeResourceInfo

This library builds on [`Deltics.PeImageInfo`](https://), providing information about resources contained in a PE image.

The `ResourceInfo` constructor accepts a stream which is passed to an internal `PeImage` object.  The `Reader` exposed by the `PeImage` is then used to locate and read the contents of any `.rsrc` section in the PE Image.

The resource information in the PE image is presented as raw `Image Resource` data structures which can then be interpreted directly for access to specific resources.

Alternatively, methods are provided for retrieving resource by reference to a `ResourceType` and `Id`.

The `Deltics.PeVersionInfo` library uses this resource info library to identify and load any version info resource and provides an example:

```
   var resources = new ResourceInfo(new FileStream('somefile.exe", FileMode.Open))
   var resource = resources.GetResource(ResourceType.VERSION, 1);
```

**NOTE:** _The `GetResource` method returns an `ImageResourceData` object that identifies the associated resource data in the resource section.  How this data is interpreted into a form that usefully describes the particular resource is a separate concern, dealt with by libraries such as `Deltics.PeVersionInfo` that understand the resource data concerned_   