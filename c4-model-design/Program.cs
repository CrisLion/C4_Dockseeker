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
            const long workspaceId = 77509; //Cambiar el workspaceID de acuerdo a su workspace
            const string apiKey = "dc12db74-80e1-4ef7-b303-3eb2e4894a63"; // cambiar el apiKey de acuerdo a su workspace 
            const string apiSecret = "54b717ba-4040-4d8b-a77d-769f812d86c6"; // cambiar el apiSecret de Acuerdo a su workspace

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
            Person admin = model.AddPerson("DocSeeker Admin", "An app user with master functions");

            patient.Uses(docSeekerSystem, "Search for doctors and book medical appointments using");
            doctor.Uses(docSeekerSystem, "Search for patients and offer his services");
            admin.Uses(docSeekerSystem, "Moderate the behavior of the users");

            docSeekerSystem.Uses(paymentGatewaySystem, "Makes payments using");
            docSeekerSystem.Uses(emailSystem, "Sends e-mail using");

            // Tags
            patient.AddTags("user");
            doctor.AddTags("user");
            admin.AddTags("user");
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
            admin.Uses(mobileApplication, "Query");

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

            // 3. Diagrama de Componentes (Security Context)
            
            Component signInController = securityContext.AddComponent("Sign In Controller", "Permite al usuario acceder a su cuenta personal.", "NodeJS (NestJS)");
            Component securityComponent = securityContext.AddComponent("Security Component", "Funcionalidad de seguridad para Sign In y contraseñas.", "NodeJS (NestJS)");

            apiRest.Uses(signInController, "");
            apiRest.Uses(apiRest, "");
            signInController.Uses(securityComponent, "");
            securityComponent.Uses(database, "");

            //Tags
            signInController.AddTags("SignInController");
            securityComponent.AddTags("SecurityComponent");

            styles.Add(new ElementStyle("SignInController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("SecurityComponent") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            ComponentView componentView = viewSet.CreateComponentView(securityContext, "Component", "Component Diagrams");
            componentView.PaperSize = PaperSize.A4_Landscape;
            componentView.Add(apiRest);
            componentView.Add(signInController);
            componentView.Add(securityComponent);
            componentView.Add(database);

            

            // 4. Diagrama de Componentes (Monitoring Context)
            Component monitoringController = monitoringContext.AddComponent("Monitoring Controller");
            Component emailNotification = monitoringContext.AddComponent("E-mail Notification");
            Component doctorRepository = monitoringContext.AddComponent("Doctor Repository");
            Component patientRepository = monitoringContext.AddComponent("Patient Repository");
            Component appointmentRepository = monitoringContext.AddComponent("Appointment Repository");
            Component paymentFacade = monitoringContext.AddComponent("Payment System Facade");

            apiRest.Uses(monitoringController, "Uses");
            apiRest.Uses(paymentFacade, "");
            apiRest.Uses(emailNotification, "");

            monitoringController.Uses(patientRepository, "");
            monitoringController.Uses(doctorRepository, "");
            monitoringController.Uses(appointmentRepository, "");

            paymentFacade.Uses(paymentGatewaySystem, "");
            emailNotification.Uses(emailSystem, "");

            patientRepository.Uses(database, "");
            doctorRepository.Uses(database, "");
            appointmentRepository.Uses(database, "");

            //Tags
            emailNotification.AddTags("EmailNotification");
            doctorRepository.AddTags("DoctorRepository");
            monitoringController.AddTags("MonitoringController");
            patientRepository.AddTags("PatientRepository");
            appointmentRepository.AddTags("AppointmentRepository");
            paymentFacade.AddTags("PaymentFacade");

            styles.Add(new ElementStyle("EmailNotification") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("DoctorRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("MonitoringController") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("PatientRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("AppointmentRepository") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
            styles.Add(new ElementStyle("PaymentFacade") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });

            ComponentView componentViewB = viewSet.CreateComponentView(monitoringContext, "Components", "Component Diagram");
            componentViewB.PaperSize = PaperSize.A4_Landscape;
            componentViewB.AddAllComponents();
            componentViewB.Add(apiRest);
            componentViewB.Add(database);
            componentViewB.Add(emailSystem);
            componentViewB.Add(paymentGatewaySystem);

            structurizrClient.UnlockWorkspace(workspaceId);
            structurizrClient.PutWorkspace(workspaceId, workspace);
        }
    }
}
