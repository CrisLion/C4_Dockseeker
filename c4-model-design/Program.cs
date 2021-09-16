﻿using Structurizr;
using Structurizr.Api;

namespace c4_model_design
{
    class Program
    {
        static void Main(string[] args)
        {
            Banking();
        }

        static void Banking()
        {
            const long workspaceId = 0;
            const string apiKey = "";
            const string apiSecret = "";

            StructurizrClient structurizrClient = new StructurizrClient(apiKey, apiSecret);
            Workspace workspace = new Workspace("Design - C4 Model - Sistema de Monitoreo", "Sistema de Monitoreo del Traslado Aéreo de Vacunas SARS-CoV-2");
            Model model = workspace.Model;

            SoftwareSystem monitoringSystem = model.AddSoftwareSystem("Monitoreo del Traslado Aéreo de Vacunas SARS-CoV-2", "Permite el seguimiento y monitoreo del traslado aéreo a nuestro país de las vacunas para la COVID-19.");
            SoftwareSystem googleMaps = model.AddSoftwareSystem("Google Maps", "Plataforma que ofrece una REST API de información geo referencial.");
            SoftwareSystem aircraftSystem = model.AddSoftwareSystem("Aircraft System", "Permite transmitir información en tiempo real por el avión del vuelo a nuestro sistema");

            Person ciudadano = model.AddPerson("Ciudadano", "Ciudadano peruano.");
            
            ciudadano.Uses(monitoringSystem, "Realiza consultas para mantenerse al tanto de la planificación de los vuelos hasta la llegada del lote de vacunas al Perú");
            monitoringSystem.Uses(aircraftSystem, "Consulta información en tiempo real por el avión del vuelo");
            monitoringSystem.Uses(googleMaps, "Usa");

            ViewSet viewSet = workspace.Views;

            // 1. Diagrama de Contexto
            SystemContextView contextView = viewSet.CreateSystemContextView(monitoringSystem, "Contexto", "Diagrama de contexto");
            contextView.PaperSize = PaperSize.A4_Landscape;
            contextView.AddAllSoftwareSystems();
            contextView.AddAllPeople();

            // Tags
            monitoringSystem.AddTags("SistemaMonitoreo");
            googleMaps.AddTags("GoogleMaps");
            aircraftSystem.AddTags("AircraftSystem");
            ciudadano.AddTags("Ciudadano");

            Styles styles = viewSet.Configuration.Styles;
            styles.Add(new ElementStyle("Ciudadano") { Background = "#0a60ff", Color = "#ffffff", Shape = Shape.Person });
            styles.Add(new ElementStyle("SistemaMonitoreo") { Background = "#008f39", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle("GoogleMaps") { Background = "#90714c", Color = "#ffffff", Shape = Shape.RoundedBox });
            styles.Add(new ElementStyle("AircraftSystem") { Background = "#2f95c7", Color = "#ffffff", Shape = Shape.RoundedBox });

            // 2. Diagrama de Contenedores
            Container mobileApplication = monitoringSystem.AddContainer("Mobile App", "Permite a los usuarios visualizar un dashboard con el resumen de toda la información del traslado de los lotes de vacunas.", "Flutter");
            Container webApplication = monitoringSystem.AddContainer("Web App", "Permite a los usuarios visualizar un dashboard con el resumen de toda la información del traslado de los lotes de vacunas.", "Flutter Web");
            Container landingPage = monitoringSystem.AddContainer("Landing Page", "", "Flutter Web");
            Container apiRest = monitoringSystem.AddContainer("API Rest", "API Rest", "Spring Boot port 8080");
            Container flightPlanningContext = monitoringSystem.AddContainer("Flight Planning Context", "Bounded Context del Microservicio de Planificación de Vuelos", "Spring Boot port 8081");
            Container airportContext = monitoringSystem.AddContainer("Airport Context", "Bounded Context del Microservicio de información de Aeropuertos", "Spring Boot port 8082");
            Container aircraftInventoryContext = monitoringSystem.AddContainer("Aircraft Inventory Context", "Bounded Context del Microservicio de Inventario de Aviones", "Spring Boot port 8083");
            Container vaccinesInventoryContext = monitoringSystem.AddContainer("Vaccines Inventory Context", "Bounded Context del Microservicio de Inventario de Vacunas", "Spring Boot port 8084");
            Container monitoringContext = monitoringSystem.AddContainer("Monitoring Context", "Bounded Context del Microservicio de Monitoreo en tiempo real del status y ubicación del vuelo que transporta las vacunas", "Spring Boot port 8085");
            Container database = monitoringSystem.AddContainer("Database", "", "Oracle");
            
            ciudadano.Uses(mobileApplication, "Consulta");
            ciudadano.Uses(webApplication, "Consulta");
            ciudadano.Uses(landingPage, "Consulta");

            mobileApplication.Uses(apiRest, "API Request", "JSON/HTTPS");
            webApplication.Uses(apiRest, "API Request", "JSON/HTTPS");

            apiRest.Uses(flightPlanningContext, "API Request", "JSON/HTTPS");
            apiRest.Uses(airportContext, "API Request", "JSON/HTTPS");
            apiRest.Uses(aircraftInventoryContext, "API Request", "JSON/HTTPS");
            apiRest.Uses(vaccinesInventoryContext, "API Request", "JSON/HTTPS");
            apiRest.Uses(monitoringContext, "API Request", "JSON/HTTPS");
            
            flightPlanningContext.Uses(database, "", "JDBC");
            airportContext.Uses(database, "", "JDBC");
            aircraftInventoryContext.Uses(database, "", "JDBC");
            vaccinesInventoryContext.Uses(database, "", "JDBC");
            monitoringContext.Uses(database, "", "JDBC");
            monitoringContext.Uses(googleMaps, "API Request", "JSON/HTTPS");
            monitoringContext.Uses(aircraftSystem, "API Request", "JSON/HTTPS");

            // Tags
            mobileApplication.AddTags("MobileApp");
            webApplication.AddTags("WebApp");
            landingPage.AddTags("LandingPage");
            apiRest.AddTags("APIRest");
            database.AddTags("Database");
            flightPlanningContext.AddTags("FlightPlanningContext");
            airportContext.AddTags("AirportContext");
            aircraftInventoryContext.AddTags("AircraftInventoryContext");
            vaccinesInventoryContext.AddTags("VaccinesInventoryContext");
            monitoringContext.AddTags("MonitoringContext");

            styles.Add(new ElementStyle("MobileApp") { Background = "#9d33d6", Color = "#ffffff", Shape = Shape.MobileDevicePortrait, Icon = "" });
            styles.Add(new ElementStyle("WebApp") { Background = "#9d33d6", Color = "#ffffff", Shape = Shape.WebBrowser, Icon = "" });
            styles.Add(new ElementStyle("LandingPage") { Background = "#929000", Color = "#ffffff", Shape = Shape.WebBrowser, Icon = "" });
            styles.Add(new ElementStyle("APIRest") { Shape = Shape.RoundedBox, Background = "#0000ff", Color = "#ffffff", Icon = "" });
            styles.Add(new ElementStyle("Database") { Shape = Shape.Cylinder, Background = "#ff0000", Color = "#ffffff", Icon = "" });
            styles.Add(new ElementStyle("MessageBus") { Width = 850, Background = "#fd8208", Color = "#ffffff", Shape = Shape.Pipe, Icon = "" });
            styles.Add(new ElementStyle("FlightPlanningContext") { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("AirportContext") { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("AircraftInventoryContext") { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("VaccinesInventoryContext") { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("MonitoringContext") { Shape = Shape.Hexagon, Background = "#facc2e", Icon = "" });

            ContainerView containerView = viewSet.CreateContainerView(monitoringSystem, "Contenedor", "Diagrama de contenedores");
            contextView.PaperSize = PaperSize.A4_Landscape;
            containerView.AddAllElements();

            // 3. Diagrama de Componentes
            Component monitoringController = monitoringContext.AddComponent("Monitoring Controller", "REST API endpoints de monitoreo.", "Spring Boot REST Controller");
            Component monitoringApplicationService = monitoringContext.AddComponent("Monitoring Application Service", "Provee métodos para el monitoreo, pertenece a la capa Application de DDD", "Spring Component");
            Component flightRepository = monitoringContext.AddComponent("Flight Repository", "Información del vuelo", "Spring Component");
            Component vaccineLoteRepository = monitoringContext.AddComponent("VaccineLote Repository", "Información de lote de vacunas", "Spring Component");
            Component locationRepository = monitoringContext.AddComponent("Location Repository", "Ubicación del vuelo", "Spring Component");
            Component aircraftSystemFacade = monitoringContext.AddComponent("Aircraft System Facade", "", "Spring Component");

            // Tags
            monitoringController.AddTags("MonitoringController");
            monitoringApplicationService.AddTags("MonitoringApplicationService");
            flightRepository.AddTags("FlightRepository");
            vaccineLoteRepository.AddTags("VaccineLoteRepository");
            locationRepository.AddTags("LocationRepository");
            aircraftSystemFacade.AddTags("AircraftSystemFacade");
            
            styles.Add(new ElementStyle("MonitoringController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("MonitoringApplicationService") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("MonitoringDomainModel") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("FlightStatus") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("FlightRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("VaccineLoteRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("LocationRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("AircraftSystemFacade") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            
            apiRest.Uses(monitoringController, "", "JSON/HTTPS");
            monitoringController.Uses(monitoringApplicationService, "Invoca métodos de monitoreo");
            monitoringController.Uses(aircraftSystemFacade, "Usa");
            monitoringApplicationService.Uses(flightRepository, "", "JDBC");
            monitoringApplicationService.Uses(vaccineLoteRepository, "", "JDBC");
            monitoringApplicationService.Uses(locationRepository, "", "JDBC");
            flightRepository.Uses(database, "", "JDBC");
            vaccineLoteRepository.Uses(database, "", "JDBC");
            locationRepository.Uses(database, "", "JDBC");
            locationRepository.Uses(googleMaps, "", "JSON/HTTPS");
            aircraftSystemFacade.Uses(aircraftSystem, "JSON/HTTPS");
            
            ComponentView componentView = viewSet.CreateComponentView(monitoringContext, "Components", "Component Diagram");
            componentView.PaperSize = PaperSize.A4_Landscape;
            componentView.Add(mobileApplication);
            componentView.Add(webApplication);
            componentView.Add(apiRest);
            componentView.Add(database);
            componentView.Add(aircraftSystem);
            componentView.Add(googleMaps);
            componentView.AddAllComponents();

            structurizrClient.UnlockWorkspace(workspaceId);
            structurizrClient.PutWorkspace(workspaceId, workspace);
        }
    }
}