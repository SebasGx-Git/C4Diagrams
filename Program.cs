using System;
using Structurizr;
using Structurizr.Api;


namespace C4Model
{
    class Program
    {
        static void Main(string[] args)
        {
            Banking();
        }

        static void Banking() {

            const long workspaceId = 73432;
            const string apiKey = "eb98e621-fdf7-4ba6-8b4f-10deab130545";
            const string apiSecret = "a8016dca-1d15-4966-a280-5299215bf806";

            StructurizrClient structurizrClient = new StructurizrClient(apiKey, apiSecret);
            Workspace workspace = new Workspace("PERUSTARS", "Sistema de publicación de información sobre Obras de Artes y eventos en Peru");
            Model model = workspace.Model;
            ViewSet viewSet = workspace.Views;

            //1. Diagrama de Contexto

            SoftwareSystem peruStars = model.AddSoftwareSystem("PeruStars", "Plataforma que permite a los artistas publicar informacion sobre sus obras y eventos proximos. Ademas de la creación de calendarios y organización para los aficionados");
            SoftwareSystem googleMaps = model.AddSoftwareSystem("Google Maps", "Plataforma que ofrece una REST API de información geo referencial.");
            SoftwareSystem googleCalendar = model.AddSoftwareSystem("Google Calendar", "Plataforma que permite crear horarios de diferentes eventos. Este API permite que el evento se registre en esta plataforma");
            SoftwareSystem zoom = model.AddSoftwareSystem("Zoom", "Esta plataforma, mediante su API, permite a nuestros artistas crear charlas y conversatorios");

            Person artista = model.AddPerson("Artistas", "Artistas peruanos en cualquier ambito ");
            Person aficionado = model.AddPerson("Aficionados", "Personas que gustan del arte");

            //Relaciones

            artista.Uses(peruStars,"Publica información sobre sus obras de artes ,eventos y organiza charlas atravez de esta plataforma ");
            aficionado.Uses(peruStars,"Realiza consultas sobre las obras de artes y eventos en Peru. Ademas de añadir estos eventos a un calendario");
            peruStars.Uses(googleMaps, "Muestra referencia geografica sobre la localización de un evento");
            peruStars.Uses(googleCalendar, "Los aficionados agregan sus eventos favoritos a esta plataforma externa");
            peruStars.Uses(zoom, "Los artistas crean conversatorios en esta plataforma externa");

            SystemContextView contextView = viewSet.CreateSystemContextView(peruStars, "Contexto", "Diagrama de contexto");
            contextView.PaperSize = PaperSize.A4_Landscape;
            contextView.AddAllSoftwareSystems();
            contextView.AddAllPeople();

            //Tags

            artista.AddTags("Artista");
            aficionado.AddTags("Aficionado");
            peruStars.AddTags("PeruStars");
            googleMaps.AddTags("GoogleMaps");
            googleCalendar.AddTags("GoogleCalendar");
            zoom.AddTags("Zoom");

            //Styles
            Styles styles = viewSet.Configuration.Styles;
            styles.Add(new ElementStyle("Artista") { Background = "#0a60ff", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle("Aficionado") { Background = "#0a60ff", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle("PeruStars") { Background = "#FF5733", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle("GoogleMaps") { Background = "#D25CDE", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle("GoogleCalendar") { Background = "#D25CDE", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle("Zoom") { Background = "#D25CDE", Color = "#ffffff", Shape = Shape.RoundedBox });



            // 2.Diagrama de Contenedores

            Container mobileApplication = peruStars.AddContainer("Mobile App", "Permite a los usuarios visualizar las obras de arte y eventos de artistas peruanos. Además permite a los artistas publicar información sobre esta", "Flutter");
            Container webApplication = peruStars.AddContainer("Permite a los usuarios visualizar las obras de arte y eventos de artistas peruanos. Además permite a los artistas publicar información sobre esta", "Flutter Web");
            Container landingPage = peruStars.AddContainer("Landing Page", "Plataforma donde se mostrara la información basica sobre nuestra plataforma", "Flutter Web");
            Container apiGateway = peruStars.AddContainer("API Gateway", "API Gateway", "Spring Boot port 8080");

            Container artistContext = peruStars.AddContainer("Artist Context", "Bounded context del Microservicio del Artista", "Spring Boot port 8081");
            Container hobbyistContext = peruStars.AddContainer("Hobbyist Context", "Bounded context del Microservicio del Aficionado", "Spring Boot port 8082");
            Container artworkContext = peruStars.AddContainer("Artwork Context", "Bounded context del Microservicio de las Obras de arte", "Spring Boot port 8083");
            Container eventContext = peruStars.AddContainer("Event Context", "Bounded context del Microservicio de los Eventos", "Spring Boot port 8084");
            Container reportContext = peruStars.AddContainer("Report Context", "Bounded context del Microservicio de los reportes", "Spring Boot port 8085");

            Container messageBus =
               peruStars.AddContainer("Bus de Mensajes en Cluster de Alta Disponibilidad", "Transporte de eventos del dominio.", "RabbitMQ");


            Container artistContextDatabase = peruStars.AddContainer("Artist Context DB ", "", "Oracle");
            Container hobbyistContextDatabase = peruStars.AddContainer("Hobbyist Context DB ", "", "Oracle");
            Container artworkContextDatabase = peruStars.AddContainer("Artwork Context DB ", "", "Oracle");
            Container eventContextDatabase = peruStars.AddContainer("Event Context DB ", "", "Oracle");
            Container reportContextDatabase = peruStars.AddContainer("Report Context DB ", "", "Oracle");

            //relaciones

            artista.Uses(mobileApplication, "Consulta");
            artista.Uses(webApplication, "Consulta");
            artista.Uses(landingPage, "Consulta");

            aficionado.Uses(mobileApplication, "Consulta");
            aficionado.Uses(webApplication, "Consulta");
            aficionado.Uses(landingPage, "Consulta");

            mobileApplication.Uses(apiGateway, "API Request", "JSON/HTTPS");
            webApplication.Uses(apiGateway, "API Request", "JSON/HTTPS");

            apiGateway.Uses(artistContext, "API Request", "JSON/HTTPS");
            apiGateway.Uses(hobbyistContext, "API Request", "JSON/HTTPS");
            apiGateway.Uses(artworkContext, "API Request", "JSON/HTTPS");
            apiGateway.Uses(eventContext, "API Request", "JSON/HTTPS");
            apiGateway.Uses(reportContext, "API Request", "JSON/HTTPS");

            artistContext.Uses(messageBus, "Publica y consume eventos del dominio");
            hobbyistContext.Uses(messageBus, "Publica y consume eventos del dominio");
            artworkContext.Uses(messageBus, "Publica y consume eventos del dominio");
            eventContext.Uses(messageBus, "Publica y consume eventos del dominio");
            reportContext.Uses(messageBus, "Publica y consume eventos del dominio");

            artistContext.Uses(artistContextDatabase, "", "JDBC");
            hobbyistContext.Uses(hobbyistContextDatabase, "", "JDBC");
            artworkContext.Uses(artworkContextDatabase, "", "JDBC");
            eventContext.Uses(eventContextDatabase, "", "JDBC");
            reportContext.Uses(reportContextDatabase, "", "JDBC");

            eventContext.Uses(googleMaps, "API Request", "JSON/HTTPS");
            hobbyistContext.Uses(googleCalendar, "API Request", "JSON/HTTPS");
            eventContext.Uses(zoom, "API Request", "JSON/HTTPS");

            //Tags
            mobileApplication.AddTags("MobileApp");
            webApplication.AddTags("WebApp");
            landingPage.AddTags("LandingPage");
            apiGateway.AddTags("APIGateway");
            messageBus.AddTags("MessageBus");

            artistContext.AddTags("ArtistContext");
            artistContextDatabase.AddTags("ArtistContextDatabase");
            hobbyistContext.AddTags("HobbyistContext");
            hobbyistContextDatabase.AddTags("HobbyistContextDatabase");
            artworkContext.AddTags("ArtworkContext");
            artistContextDatabase.AddTags("ArtworkContextDatabase");
            eventContext.AddTags("EventContext");
            eventContextDatabase.AddTags("EventContextDatabase");
            reportContext.AddTags("ReportContext");
            reportContextDatabase.AddTags("ReportContextDatabase");

            styles.Add(new ElementStyle("MobileApp") { Background = "#32E1DC", Color = "#ffffff", Shape = Shape.MobileDevicePortrait, Icon = "" });
            styles.Add(new ElementStyle("WebApp") { Background = "#32E1DC", Color = "#ffffff", Shape = Shape.WebBrowser, Icon = "" });
            styles.Add(new ElementStyle("LandingPage") { Background = "#32E1DC", Color = "#ffffff", Shape = Shape.WebBrowser, Icon = "" });
            styles.Add(new ElementStyle("APIGateway") { Shape = Shape.RoundedBox, Background = "#FF5733", Color = "#ffffff", Icon = "" });

            styles.Add(new ElementStyle("ArtistContext") { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("HobbyistContext") { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("ArtworkContext") { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("EventContext") { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("ReportContext") { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });

            styles.Add(new ElementStyle("ArtistContextDatabase") { Shape = Shape.Cylinder, Background = "#ff0000", Color = "#ffffff", Icon = "" });
            styles.Add(new ElementStyle("HobbyistContextDatabase") { Shape = Shape.Cylinder, Background = "#ff0000", Color = "#ffffff", Icon = "" });
            styles.Add(new ElementStyle("ArtworkContextDatabase") { Shape = Shape.Cylinder, Background = "#ff0000", Color = "#ffffff", Icon = "" });
            styles.Add(new ElementStyle("EventContextDatabase") { Shape = Shape.Cylinder, Background = "#ff0000", Color = "#ffffff", Icon = "" });
            styles.Add(new ElementStyle("ReportContextDatabase"){ Shape = Shape.Cylinder, Background = "#ff0000", Color = "#ffffff", Icon = "" });

            styles.Add(new ElementStyle("MessageBus") { Width = 850, Background = "#fd8208", Color = "#ffffff", Shape = Shape.Pipe, Icon = "" });


            ContainerView containerView = viewSet.CreateContainerView(peruStars, "Contenedor", "Diagrama de contenedores");
            contextView.PaperSize = PaperSize.A4_Landscape;
            containerView.AddAllElements();


            //Render

            structurizrClient.UnlockWorkspace(workspaceId);
            structurizrClient.PutWorkspace(workspaceId, workspace);
        }


    }
}
