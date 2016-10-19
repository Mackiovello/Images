# Images

Put an image on anything. Supports drag'n'drop. Try it with **people** or **products**.

 **Note:** the application has been migrated to Polymer 1.x.
- Latest Polymer 0.5 commit: https://github.com/Polyjuice/Images/commit/324dee2e78bb5aaef8863270d597332215078be3
- Latest Polymer 0.5 release: https://github.com/Polyjuice/Images/releases/tag/2.0.4

## Partials

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
