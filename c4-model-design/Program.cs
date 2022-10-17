using Structurizr;
using Structurizr.Api;

namespace c4_model_design
{
    class Program
    {
        static void Main(string[] args)
        {
            RenderModels();
        }

        static void RenderModels() 
        {
            const long workspaceId = 77511; //Cambiar el workspaceID de acuerdo a su workspace
            const string apiKey = "9c2d9d62-21d9-4a5e-aac8-e866689e1728"; // cambiar el apiKey de acuerdo a su workspace 
            const string apiSecret = "aa3f580e-bfbd-420e-a3ba-1763ccd2e29e"; // cambiar el apiSecret de Acuerdo a su workspace

            StructurizrClient structurizrClient = new StructurizrClient(apiKey, apiSecret);

            Workspace workspace = new Workspace("Grupo 3: MIRAI - Producto: DocSeeker", "Sistema de servicio de atención médica a domicilio");

            ViewSet viewSet = workspace.Views;

            Model model = workspace.Model;

            // 1. Diagrama de Contexto
            SoftwareSystem docSeekerSystem = model.AddSoftwareSystem("DocSeeker System", "Allows users to find medical appointments.");
            SoftwareSystem paymentGatewaySystem = model.AddSoftwareSystem("Payment Gateway System", "Allows the users to make payments.");
            SoftwareSystem emailSystem = model.AddSoftwareSystem("E-mail System", "The internal Microsoft Exchange e-mail system.");

            Person patient = model.AddPerson("DocSeeker Patient", "An app user with a registered account");
            Person doctor = model.AddPerson("DocSeeker Doctor", "A doctor with a registered account");

            patient.Uses(docSeekerSystem, "Search for doctors and book medical appointments using");
            doctor.Uses(docSeekerSystem, "Search for patients and offer his services");

            docSeekerSystem.Uses(paymentGatewaySystem, "Makes payments using");
            docSeekerSystem.Uses(emailSystem, "Sends e-mail using");

            // Tags
            patient.AddTags("user");
            doctor.AddTags("user");
            docSeekerSystem.AddTags("docSeekerSystem");
            paymentGatewaySystem.AddTags("paymentGatewaySystem");
            emailSystem.AddTags("emailSystem");

            Styles styles = viewSet.Configuration.Styles;
            styles.Add(new ElementStyle("user") { Background = "#0a60ff", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle("docSeekerSystem") { Background = "#008f39", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle("paymentGatewaySystem") { Background = "#90714c", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle("emailSystem") { Background = "#2f95c7", Color = "#ffffff", Shape = Shape.RoundedBox });

            SystemContextView contextView = viewSet.CreateSystemContextView(docSeekerSystem, "Context", "Context Diagram");
            contextView.PaperSize = PaperSize.A5_Landscape;
            contextView.AddAllSoftwareSystems();
            contextView.AddAllPeople();

            // 2. Diagrama de Contenedores
            Container mobileApplication = docSeekerSystem.AddContainer("Mobile App", "Permite a los usuarios visualizar un dashboard con el resumen de toda la información del traslado de los lotes de vacunas.", "Swift UI");
            Container apiRest = docSeekerSystem.AddContainer("API REST", "API Rest", "NodeJS (NestJS) port 8080");

            Container monitoringContext = docSeekerSystem.AddContainer("Monitoring Context", "Bounded Context of Real-time monitoring of the payment process and notifications.", "NodeJS (NestJS)");
            Container securityContext = docSeekerSystem.AddContainer("Security Context", "Bounded Context de Seguridad", "NodeJS (NestJS)");

            Container database = docSeekerSystem.AddContainer("Database", "", "Oracle");

            patient.Uses(mobileApplication, "Query");
            doctor.Uses(mobileApplication, "Query");

            mobileApplication.Uses(apiRest, "API Request", "JSON/HTTPS");

            apiRest.Uses(monitoringContext, "", "");
            apiRest.Uses(securityContext, "", "");

            monitoringContext.Uses(database, "", "");
            securityContext.Uses(database, "", "");

            monitoringContext.Uses(paymentGatewaySystem, "API Request", "JSON/HTTPS");
            monitoringContext.Uses(emailSystem, "API Request", "JSON/HTTPS");

            // Tags
            mobileApplication.AddTags("MobileApp");
            apiRest.AddTags("APIRest");
            database.AddTags("Database");

            string contextTag = "Context";

            monitoringContext.AddTags(contextTag);
            securityContext.AddTags(contextTag);

            styles.Add(new ElementStyle("MobileApp") { Background = "#9d33d6", Color = "#ffffff", Shape = Shape.MobileDevicePortrait, Icon = "" });
            styles.Add(new ElementStyle("APIRest") { Shape = Shape.RoundedBox, Background = "#0000ff", Color = "#ffffff", Icon = "" });
            styles.Add(new ElementStyle("Database") { Shape = Shape.Cylinder, Background = "#ff0000", Color = "#ffffff", Icon = "" });
            styles.Add(new ElementStyle(contextTag) { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });

            ContainerView containerView = viewSet.CreateContainerView(docSeekerSystem, "Container", "Container Diagrams");
            contextView.PaperSize = PaperSize.A4_Landscape;
            containerView.AddAllElements();

            // 3. Diagrama de Componentes (Monitoring Context)
            /*
            Component domainLayer = monitoringContext.AddComponent("Domain Layer", "", "NodeJS (NestJS)");

            Component monitoringController = monitoringContext.AddComponent("MonitoringController", "REST API endpoints de monitoreo.", "NodeJS (NestJS) REST Controller");

            Component monitoringApplicationService = monitoringContext.AddComponent("MonitoringApplicationService", "Provee métodos para el monitoreo, pertenece a la capa Application de DDD", "NestJS Component");

            Component flightRepository = monitoringContext.AddComponent("FlightRepository", "Información del vuelo", "NestJS Component");
            Component vaccineLoteRepository = monitoringContext.AddComponent("VaccineLoteRepository", "Información de lote de vacunas", "NestJS Component");
            Component locationRepository = monitoringContext.AddComponent("LocationRepository", "Ubicación del vuelo", "NestJS Component");

            Component aircraftSystemFacade = monitoringContext.AddComponent("Aircraft System Facade", "", "NestJS Component");

            apiRest.Uses(monitoringController, "", "JSON/HTTPS");
            monitoringController.Uses(monitoringApplicationService, "Invoca métodos de monitoreo");

            monitoringApplicationService.Uses(domainLayer, "Usa", "");
            monitoringApplicationService.Uses(aircraftSystemFacade, "Usa");
            monitoringApplicationService.Uses(flightRepository, "", "");
            monitoringApplicationService.Uses(vaccineLoteRepository, "", "");
            monitoringApplicationService.Uses(locationRepository, "", "");

            flightRepository.Uses(database, "", "");
            vaccineLoteRepository.Uses(database, "", "");
            locationRepository.Uses(database, "", "");

            locationRepository.Uses(googleMaps, "", "JSON/HTTPS");

            aircraftSystemFacade.Uses(aircraftSystem, "JSON/HTTPS");
            
            // Tags
            domainLayer.AddTags("DomainLayer");
            monitoringController.AddTags("MonitoringController");
            monitoringApplicationService.AddTags("MonitoringApplicationService");
            flightRepository.AddTags("FlightRepository");
            vaccineLoteRepository.AddTags("VaccineLoteRepository");
            locationRepository.AddTags("LocationRepository");
            aircraftSystemFacade.AddTags("AircraftSystemFacade");
            
            styles.Add(new ElementStyle("DomainLayer") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("MonitoringController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("MonitoringApplicationService") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("MonitoringDomainModel") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("FlightStatus") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("FlightRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("VaccineLoteRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("LocationRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("AircraftSystemFacade") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            ComponentView componentView = viewSet.CreateComponentView(monitoringContext, "Components", "Component Diagram");
            componentView.PaperSize = PaperSize.A4_Landscape;
            componentView.Add(mobileApplication);
            componentView.Add(webApplication);
            componentView.Add(apiRest);
            componentView.Add(database);
            componentView.Add(aircraftSystem);
            componentView.Add(googleMaps);
            componentView.AddAllComponents();
            */
            structurizrClient.UnlockWorkspace(workspaceId);
            structurizrClient.PutWorkspace(workspaceId, workspace);
        }
    }
}