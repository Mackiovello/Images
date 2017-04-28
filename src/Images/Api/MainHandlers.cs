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
                    return ReturnWithinMaster(Self.GET<ImagePage>("/images/partials/image/"));
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
            UriMapping.Map("/images/menu", UriMapping.MappingUriPrefix + "/menu");
            UriMapping.Map("/images/app-name", UriMapping.MappingUriPrefix + "/app-name");
            UriMapping.Map("/images/settings", UriMapping.MappingUriPrefix + "/settings");

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
            UriMapping.OntologyMap("/images/partials/concept-somebody/{?}", typeof(Somebody).FullName, null, null);
            UriMapping.OntologyMap("/images/partials/concept-vendible/{?}", typeof(Product).FullName, null, null);
            UriMapping.OntologyMap("/images/partials/concept-chatmessage/{?}", typeof(ChatMessage).FullName, (string objectId) => objectId, (string objectId) =>
            {
                var message = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as ChatMessage;
                return message.IsDraft ? null : objectId;
            });
            UriMapping.OntologyMap("/images/partials/images-draft/{?}", typeof(ChatMessage).FullName, (string objectId) => objectId, (string objectId) =>
            {
                var chatMessage = (ChatMessage)DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId));
                return chatMessage.IsDraft ? objectId : null;
            });
            UriMapping.OntologyMap("/images/partials/concept-chatattachment/{?}", typeof(ChatAttachment).FullName, (string objectId) => objectId, (string objectId) =>
            {
                var illustration = DbHelper.FromID(DbHelper.Base64DecodeObjectID(objectId)) as Illustration;
                if (illustration == null)
                {
                    return null;
                }
                return objectId;
            });

            #region For Chatter
            UriMapping.OntologyMap("/images/partials/illustrations-edit/@w", typeof(EditAnnouncement).FullName);
            UriMapping.OntologyMap("/images/partials/illustrations/@w", typeof(PreviewAnnouncement).FullName);
            UriMapping.OntologyMap("/images/partials/imagedraftannouncement/{?}", typeof(ChatDraftAnnouncement).FullName);
            UriMapping.OntologyMap("/images/partials/imagewarning/{?}", typeof(ChatWarning).FullName);
            #endregion

            #endregion
        }
    }
}
