# Images

Put an image on anything. Supports drag'n'drop. Try it with **people** or **products**.

 **Note:** the application has been migrated to Polymer 1.x.
- Latest Polymer 0.5 commit: https://github.com/Polyjuice/Images/commit/324dee2e78bb5aaef8863270d597332215078be3
- Latest Polymer 0.5 release: https://github.com/Polyjuice/Images/releases/tag/2.0.4

## Partials

### GET /images/partials/content/`{Content ObjectID}`

Shows a simple page for `Content` preview, image or video. In case of unexisting content, shows empty file preview image.

Screenshot:

![image](https://cloud.githubusercontent.com/assets/15857369/19719114/30bed58c-9b71-11e6-8c16-99276e771f51.png)

### GET /images/partials/content-edit/`{Content ObjectID}`

Shows a simple page for `Content` preview and allows to update it with new file. 

Screenshot:

![image](https://cloud.githubusercontent.com/assets/15857369/19719008/b2e4d9c2-9b70-11e6-982b-fdfbaf2fb88f.png)

In case of unexisting content, creates new content and shows drag'n'drop upload area for new image.

Screenshot:

![image](https://cloud.githubusercontent.com/assets/15857369/19719034/d3c1fb0c-9b70-11e6-8c99-6c1b004bd5fc.png)

### GET /images/partials/illustrations/`{Something ObjectID}`

Shows a carousel for images that become `Illustration` of `Something` in read only mode. 

Screenshot:

![image](https://cloud.githubusercontent.com/assets/15857369/19719131/48234ab4-9b71-11e6-9604-1b7f63dee3b6.png)

In case of unexisting something, shows empty carousel with single empty file preview image.

Screenshot:

![image](https://cloud.githubusercontent.com/assets/15857369/19719139/56824c5e-9b71-11e6-8279-041ef7befdc3.png)

### GET /images/partials/illustrations-edit/`{Something ObjectID}`

Shows a carousel for images that become `Illustration` of `Something` with drag'n'drop upload area and button to add new illustrations. 

Screenshot:

![image](https://cloud.githubusercontent.com/assets/15857369/19719209/9af9b84a-9b71-11e6-8701-004469966328.png)

In case of unexisting something, shows empty carousel with empty drag'n'drop upload area.

Screenshot:

![image](https://cloud.githubusercontent.com/assets/15857369/19719233/b6803864-9b71-11e6-81d5-de880cdeedfc.png)

### GET /images/partials/concept/`{Something ObjectID}`

Shows a drag'n'drop upload area for images that become `Illustration` of `Something`. In case the provided something already has an illustration, shows that illustration and allows to overwrite it. In case of unexisting something, shows empty partial.

Screenshot:

![image](docs/screenshot-partial-concept.png)

Sample mapping:

```cs
StarcounterEnvironment.RunWithinApplication("Images", () => {
    Handle.GET("/images/partials/concept-YOURCLASS/{?}", (string objectId) => {
        return Self.GET("/images/partials/concept/" + objectId);
    });

    UriMapping.OntologyMap<YOURAPP.YOURCLASS>("/images/partials/concept-YOURCLASS/{?}");
});
```

## License

MIT
