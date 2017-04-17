using Starcounter;
using Simplified.Ring1;
using Simplified.Ring3;
using Simplified.Ring6;
using Simplified.Ring6.Images;

namespace Images
{
    internal class MainHandlers
    {
        public void Register()
        {
            Application.Current.Use(new HtmlFromJsonProvider());
            Application.Current.Use(new PartialToStandaloneHtmlProvider());

            Handle.GET("/images/standalone", () =>
            {
                var session = Session.Current;
                if (session?.Data != null)
                {
                    return session.Data;
                }

                var masterPage = new MasterPage();
                if (session == null)
                {
                    session = new Session(SessionOptions.PatchVersioning);
                }

                masterPage.Session = session;
                return masterPage;
            });

            // Workspace root (Launchpad)
            Handle.GET("/images", (Request request) =>
            {
                return Db.Scope(() =>
                {
                    return ReturnWithinMaster(Self.GET("/images/partials/images"));
                });
            });

            Handle.GET("/images/image", () =>
            {
                return Db.Scope<Json>(() =>
                {
                    return ReturnWithinMaster(Self.GET<ImagePage>("/Images/partials/image/"));
                });
            });

            Handle.GET("/images/image/{?}", (string objectId) =>
            {
                return Db.Scope<Json>(() =>
                {
                    return ReturnWithinMaster(Self.GET<ImagePage>("/Images/partials/image/" + objectId));
                });
            });

            Handle.GET("/images/contents/{?}", (string objectId) =>
            {
                return Db.Scope<Json>(() =>
                {
                    return ReturnWithinMaster(Self.GET<ContentPage>("/images/partials/contents/" + objectId));
                });
            });

            Handle.GET("/images/contents-edit/{?}", (string objectId) =>
            {
                return Db.Scope<Json>(() =>
                {
                    return ReturnWithinMaster(Self.GET<EditableContentPage>("/images/partials/contents-edit/" + objectId));
                });
            });

            Handle.GET("/images/somethings/{?}", (string objectId) =>
            {
                return Db.Scope<Json>(() =>
                {
                    return ReturnWithinMaster(Self.GET<IllustrationsPage>("/images/partials/somethings/" + objectId));
                });
            });

            Handle.GET("/images/somethings-edit/{?}", (string objectId) =>
            {
                return Db.Scope<Json>(() =>
                {
                    return ReturnWithinMaster(Self.GET<EditableIllustrationsPage>("/images/partials/somethings-edit/" + objectId));
                });
            });

            RegisterPartials();
            RegisterLauncherHooks();
            RegisterMapperHandlers();
        }

        private MasterPage ReturnWithinMaster(Json currentPage)
        {
            var master = Self.GET<MasterPage>("/images/standalone");
            master.CurrentPage = currentPage;
            return master;
        }
        
        protected void RegisterLauncherHooks()
        {
            Handle.GET("/images/app-name", () => new AppName());

            // Menu
            Handle.GET("/images/menu", () => new Page { Html = "/Images/viewmodels/AppMenu.html" });
        }

        protected void RegisterPartials()
        {
            Handle.GET("/images/partials/images", (Request request) =>
            {
                var page = new ImagesPage
                {
                    Html = "/Images/viewmodels/ImagesPage.html",
                    Uri = request.Uri
                };

                return page;
            });

            Handle.GET("/images/partials/somethings/{?}", (string objectId) =>
            {
                var data = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as Something;
                var page = new IllustrationsPage
                {
                    Data = data
                };
                return page;
            });

            Handle.GET("/images/partials/somethings-edit/{?}", (string objectId) =>
            {
                var data = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as Something;
                var page = new EditableIllustrationsPage
                {
                    Data = data
                };

                return page;
            });

            Handle.GET("/images/partials/illustrations/{?}", (string illustrationId) =>
            {
                var illustration = DbHelper.FromID(DbHelper.Base64DecodeObjectID(illustrationId)) as Illustration;
                if (illustration?.Content == null)
                {
                    var errorPage = new ErrorPage
                    {
                        ErrorText = "Images cannot present an illustration without content"
                    };
                    return errorPage;
                }
                return Self.GET("/images/partials/contents/" + illustration.Content.Key);
            });

            Handle.GET("/images/partials/contents/{?}", (string objectId) =>
            {
                return Db.Scope<Json>(() =>
                {
                    var content = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as Content;

                    var page = new ContentPage
                    {
                        Data = content
                    };
                    return page;
                });
            });

            Handle.GET("/images/partials/contents-edit/{?}", (string objectId) =>
            {
                return Db.Scope<Json>(() =>
                {
                    var content = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as Content;

                    if (content == null)
                    {
                        var name = "Standalone image";
                        var illustration = new Illustration
                        {
                            Concept = new Something {Name = name},
                            Content = new Content {Name = name},
                            Name = name
                        };
                        content = illustration.Content;
                    }

                    return new EditableContentPage { Data = content };
                });
            });

            Handle.GET("/images/partials/image/", () =>
            {
                var name = "Standalone image";
                var illustration = new Illustration
                {
                    Concept = new Something { Name = name },
                    Content = new Content { Name = name },
                    Name = name
                };

                var imagePage = new ImagePage
                {
                    Data = illustration.Content,
                    EditableContent = Self.GET($"/images/partials/contents-edit/{illustration.Content.Key}")
                };
                return imagePage;
            });

            Handle.GET("/images/partials/image/{?}", (string contentId) =>
            {
                var content = DbHelper.FromID(DbHelper.Base64DecodeObjectID(contentId)) as Content;
                if (content == null)
                {
                    var errorPage = new ErrorPage
                    {
                        ErrorText = "Images cannot present an illustration without content"
                    };
                    return errorPage;
                }
                var imagePage = new ImagePage
                {
                    Data = content,
                    EditableContent = Self.GET($"/images/partials/contents-edit/{contentId}")
                };
                return imagePage;
            });

            #region Custom application handlers
            Handle.GET("/images/partials/images-draft/{?}", (string chatMessageId) =>
            {
                var chatMessage = (ChatMessage)DbHelper.FromID(DbHelper.Base64DecodeObjectID(chatMessageId));
                var relation = new Illustration
                {
                    Concept = chatMessage,
                    Content = new Simplified.Ring1.Content() { Name = "Content for illustration of a chat message" },
                };
                var draft = new DraftPage
                {
                    SubPage = Self.GET("/images/partials/imagedraftannouncement/" + relation.GetObjectID())
                };
                return draft;
            });

            Handle.GET("/images/partials/somethings-single/{?}", (string objectId) =>
            {
                var message = (Something)DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId));
                var illustration = Db.SQL<Illustration>(@"Select m from Simplified.Ring1.Illustration m Where m.ToWhat = ?", message).First;
                return illustration == null ? new Page() : Self.GET("/images/partials/illustrations/" + illustration.GetObjectID());
            });

            Handle.GET("/images/partials/somethings-single-static/{?}", (string objectId) =>
            {
                var something = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as Something;
                string key = something?.Illustration?.Key;

                if (key != null)
                {
                    return new IllustrationSimplePage
                    {
                        Data = DbHelper.FromID(DbHelper.Base64DecodeObjectID(key)) as Illustration
                    };
                }

                return new Json();
            });

            Handle.GET("/images/partials/illustrations-edit/{?}", (string illustrationId) =>
            {
                var illustration = DbHelper.FromID(DbHelper.Base64DecodeObjectID(illustrationId)) as Illustration;
                if (illustration == null)
                {
                    var errorPage = new ErrorPage
                    {
                        ErrorText = "Images cannot present an empty illustration"
                    };
                    return errorPage;
                }

                if (illustration.Content == null)
                {
                    illustration.Content = new Content {Name = "Standalone image" };

                }

                return Self.GET("/images/partials/contents-edit/" + illustration.Content.Key);
            });

            Handle.GET("/images/partials/imagedraftannouncement/{?}", (string objectPath) => new Page());

            Handle.GET("/images/partials/imagewarning/{?}", (string illustrationId) =>
            {
                var illustration = DbHelper.FromID(DbHelper.Base64DecodeObjectID(illustrationId)) as Illustration;
                if (illustration == null) return new Page();

                var page = new ConceptIllustrationWarningPage();
                page.RefreshData(illustration);
                return page;
            });
            #endregion

            UploadHandlers.GET("/images/images", task =>
            {
                Session.ScheduleTask(task.SessionId, (s, id) =>
                {
                    s.CalculatePatchAndPushOnWebSocket();
                });
            });
            Handle.GET("/images/settings", () =>
            {
                return new Transaction().Scope(() =>
                {
                    var page = new SettingsPage();
                    page.LoadDefaultData();
                    return page;
                });
            }, new HandlerOptions { SelfOnly = true });
        }

        protected void RegisterMapperHandlers()
        {
            Blender.MapUri("/images/menu", "menu");
            Blender.MapUri("/images/app-name", "app-name");
            Blender.MapUri("/images/settings", "settings");

            #region Wrapper URI handlers for usage in OntologyMap
            Handle.GET("/images/partials/concept-somebody/{?}", (string objectId) =>
            {
                return Self.GET("/images/partials/somethings-edit/" + objectId);
            });
            Handle.GET("/images/partials/concept-vendible/{?}", (string objectId) =>
            {
                return Self.GET("/images/partials/somethings-edit/" + objectId);
            });
            Handle.GET("/images/partials/concept-chatmessage/{?}", (string objectId) =>
            {
                return Self.GET("/images/partials/somethings-single/" + objectId);
            });
            Handle.GET("/images/partials/concept-chatattachment/{?}", (string objectId) =>
            {
                return Self.GET("/images/partials/illustrations-edit/" + objectId);
            });
            # endregion

            #region OntologyMap
            // TODO: This mapping can be removed since there is no app which uses mapping on Somebody class
            Blender.MapUri<Somebody>("/images/partials/concept-somebody/{?}");

            Blender.MapUri<Product>("/images/partials/concept-vendible/{?}");
            Blender.MapUri<ChatMessage>("/images/partials/concept-chatmessage/{?}",
                paramsFrom => paramsFrom,
                paramsTo =>
                {
                    var objectId = paramsTo[0];
                    var message = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as ChatMessage;
                    return message.IsDraft ? null : paramsTo;
                });
            Blender.MapUri<ChatMessage>("/images/partials/images-draft/{?}",
                paramsFrom => paramsFrom,
                paramsTo =>
                {
                    var objectId = paramsTo[0];
                    var chatMessage = (ChatMessage)DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId));
                    return chatMessage.IsDraft ? paramsTo : null;
                });
            Blender.MapUri<ChatAttachment>("/images/partials/concept-chatattachment/{?}",
                paramsFrom => paramsFrom,
                paramsTo =>
                {
                    var objectId = paramsTo[0];
                    var illustration = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as Illustration;
                    if (illustration == null)
                    {
                        return null;
                    }
                    return paramsTo;
                });

            #region For Chatter
            Blender.MapUri<EditAnnouncement>("/images/partials/illustrations-edit/{?}");
            Blender.MapUri<PreviewAnnouncement>("/images/partials/illustrations/{?}");
            Blender.MapUri<ChatDraftAnnouncement>("/images/partials/imagedraftannouncement/{?}");
            Blender.MapUri<ChatWarning>("/images/partials/imagewarning/{?}");
            #endregion

            #endregion
        }
    }
}
