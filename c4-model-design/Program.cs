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
            Container database = docSeekerSystem.AddContainer("Database", "", "Oracle");

            //Modulos
            Container reviewContext = docSeekerSystem.AddContainer("Review Context", "Bounded Context of Real-time monitoring of the payment process and notifications.", "NodeJS (NestJS)");
            Container securityContext = docSeekerSystem.AddContainer("Security Context", "Bounded Context de Seguridad", "NodeJS (NestJS)");
            Container notificationContext = docSeekerSystem.AddContainer("Notification Context", "Bounded Context de Notrificaciones", "NodeJS (NestJS)");
            Container PaymentContext = docSeekerSystem.AddContainer("Payment Context", "Payment Context de pagos", "NodeJS (NestJS)");
            Container AppointmentContext = docSeekerSystem.AddContainer("Appointment Context", "Appointment Context de citas", "NodeJS (NestJS)");
            Container PrescriptionContext = docSeekerSystem.AddContainer("Prescription Context", "Prescription Context", "NodeJS (NestJS)");

            patient.Uses(mobileApplication, "Query");
            doctor.Uses(mobileApplication, "Query");
            admin.Uses(mobileApplication, "Query");

            mobileApplication.Uses(apiRest, "API Request", "JSON/HTTPS");

            apiRest.Uses(reviewContext, "", "");
            apiRest.Uses(securityContext, "", "");
            apiRest.Uses(notificationContext, "", "");
            apiRest.Uses(PaymentContext, "", "");
            apiRest.Uses(AppointmentContext, "", "");
            apiRest.Uses(PrescriptionContext, "", "");

            reviewContext.Uses(database, "", "");
            securityContext.Uses(database, "", "");
            notificationContext.Uses(database, "", "");
            PaymentContext.Uses(database, "", "");
            AppointmentContext.Uses(database, "", "");
            PrescriptionContext.Uses(database, "", "");

            PaymentContext.Uses(paymentGatewaySystem, "API Request", "JSON/HTTPS");
            notificationContext.Uses(emailSystem, "API Request", "JSON/HTTPS");

            // Tags
            mobileApplication.AddTags("MobileApp");
            apiRest.AddTags("APIRest");
            database.AddTags("Database");

            string contextTag = "Context";

            reviewContext.AddTags(contextTag);
            securityContext.AddTags(contextTag);
            notificationContext.AddTags(contextTag);
            PaymentContext.AddTags(contextTag);
            AppointmentContext.AddTags(contextTag);
            PrescriptionContext.AddTags(contextTag);

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
            Component userVerifyComponent = securityContext.AddComponent("User Verify Repository", "Verifica la información en el repositorio de cuentas de la base de datos.", "NodeJS (NestJS)");

            apiRest.Uses(signInController, "");
            signInController.Uses(securityComponent, "uses");
            securityComponent.Uses(userVerifyComponent, "reads from");
            userVerifyComponent.Uses(database, "");

            //Tags

            string componentTag = "component";

            signInController.AddTags(componentTag);
            securityComponent.AddTags(componentTag);
            userVerifyComponent.AddTags(componentTag);

            styles.Add(new ElementStyle("component") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });
           
            ComponentView componentView = viewSet.CreateComponentView(securityContext, "Security Component", "Component Diagrams");
            componentView.PaperSize = PaperSize.A4_Landscape;
            componentView.Add(apiRest);
            componentView.AddAllComponents();
            componentView.Add(database);


            // 4. Diagrama de Componentes (Notifications  Context)
            Component notificationController = notificationContext.AddComponent("Notification Controller", "Controla las funcionalidades de las notificaciones.", "");
            Component emailNotifications = notificationContext.AddComponent("E-mail Notifications", "Servicio de envio de notificaciones al correo electronico del usuario.", "NodeJS (NestJS)");
            Component mobileNotifications = notificationContext.AddComponent("Mobile Notifications", "Funcionalidad de envio de notificaciones desde la API.", "NodeJS (NestJS)");
            Component notificationRepository = notificationContext.AddComponent("Notification Repository", "Información sobre las notificaciones.", "NodeJS (NestJS)");

            apiRest.Uses(notificationController, "");
            notificationController.Uses(emailNotifications, "uses a facade for email notifications");
            notificationController.Uses(mobileNotifications, "uses");
            emailNotifications.Uses(notificationRepository, "reads and writes");
            emailNotifications.Uses(emailSystem, "");
            mobileNotifications.Uses(notificationRepository, "reads and writes");
            notificationRepository.Uses(database, "");

            //Tags
            notificationController.AddTags(componentTag);
            emailNotifications.AddTags(componentTag);
            mobileNotifications.AddTags(componentTag);
            notificationRepository.AddTags(componentTag);

            ComponentView componentViewB = viewSet.CreateComponentView(notificationContext, "Notification Components", "Component Diagram");
            componentViewB.PaperSize = PaperSize.A4_Landscape;
            componentViewB.AddAllComponents();
            componentViewB.Add(apiRest);            
            componentViewB.Add(database);
            componentViewB.Add(emailSystem);

            // 5. Diagrama de Componentes (Payment Context)
            Component paymentController = PaymentContext.AddComponent("Payment Controller", "Controla las funcionalidades de los procesos de pago.");
            Component paymentService = PaymentContext.AddComponent("Payment Service Facade", "", "NodeJS (NestJS)");
            Component paymentRepository = PaymentContext.AddComponent("Payment Repository", "Información sobre los pagos.", "NodeJS (NestJS)");

            apiRest.Uses(paymentController, "");
            paymentController.Uses(paymentService, "uses");
            paymentService.Uses(paymentGatewaySystem, "");
            paymentService.Uses(paymentRepository, "reads and writes");
            paymentRepository.Uses(database, "");

            //Tags
            paymentController.AddTags(componentTag);
            paymentRepository.AddTags(componentTag);
            paymentService.AddTags(componentTag);

            ComponentView componentViewC = viewSet.CreateComponentView(PaymentContext, "Payment Components", "Components Diagram");
            componentViewC.PaperSize = PaperSize.A4_Landscape;
            componentViewC.AddAllComponents();
            componentViewC.Add(apiRest);
            componentViewC.Add(database);
            componentViewC.Add(paymentGatewaySystem);

            // 6. Diagrama de Componentes (Appointment Context)
            Component appointmentController = AppointmentContext.AddComponent("Appointment Controller", "Controla las funcionalidades de las citas.");
            Component appointmentRepository = AppointmentContext.AddComponent("Appointment Repository", "Información sobre las citas.", "NodeJS (NestJS)");
            Component calendarController = AppointmentContext.AddComponent("Calendar Controller");
            Component calendarRepository = AppointmentContext.AddComponent("Calendar Repository", "Información sobre el calendario y sus fechas.", "NodeJS (NestJS)");

            apiRest.Uses(appointmentController, "");
            apiRest.Uses(calendarController, "reads");
            calendarController.Uses(calendarRepository, "uses");
            appointmentController.Uses(calendarController, "writes");
            appointmentController.Uses(appointmentRepository, "");
            appointmentRepository.Uses(database, "");
            calendarRepository.Uses(database, "");

            //Tags
            appointmentRepository.AddTags(componentTag);
            appointmentController.AddTags(componentTag);
            calendarController.AddTags(componentTag);
            calendarRepository.AddTags(componentTag);

            ComponentView componentViewD = viewSet.CreateComponentView(AppointmentContext, "Appointement Components", "Components Diagram");
            componentViewD.PaperSize = PaperSize.A4_Landscape;
            componentViewD.AddAllComponents();
            componentViewD.Add(apiRest);
            componentViewD.Add(database);

            // 6. Diagrama de Componentes (Prescription Context)
            Component prescriptionController = PrescriptionContext.AddComponent("Prescription Controller", "Controla las funcionalidades de la prescripcion creada por los doctores.");
            Component diagnosisRepository = PrescriptionContext.AddComponent("Diagnosis Repository", "Información sobre la diagnosis.", "NodeJS (NestJS)");
            Component medicineRepository = PrescriptionContext.AddComponent("Medicine Repository", "Información sobre las medicinas.", "NodeJS (NestJS)");
            Component therapyRepository = PrescriptionContext.AddComponent("Therapy Repository", "Información sobre las terapias recomendadas.", "NodeJS (NestJS)");

            apiRest.Uses(prescriptionController, "");
            prescriptionController.Uses(diagnosisRepository,"reads and writes");
            prescriptionController.Uses(medicineRepository,"reads and writes");
            prescriptionController.Uses(therapyRepository,"reads and writes");
            diagnosisRepository.Uses(database, "");
            medicineRepository.Uses(database, "");
            therapyRepository.Uses(database, "");

            //Tags
            prescriptionController.AddTags(componentTag);
            diagnosisRepository.AddTags(componentTag);
            therapyRepository.AddTags(componentTag);
            medicineRepository.AddTags(componentTag);
            
            ComponentView componentViewE = viewSet.CreateComponentView(PrescriptionContext, "Prescription Components", "Components Diagram");
            componentViewE.PaperSize = PaperSize.A4_Landscape;
            componentViewE.AddAllComponents();
            componentViewE.Add(apiRest);
            componentViewE.Add(database);

            //7. Diagrama de Componentes (Review Context)
            Component reviewController = reviewContext.AddComponent("Review Controller", "Controla las funcionalidades de las reseñas a los doctores.");
            Component reviewRepository = reviewContext.AddComponent("Review Repository", "Información sobre las reseñas.", "NodeJS (NestJS)");
            Component addReviewComponent = reviewContext.AddComponent("Add Review Component", "Funcionalidad para agregar nuevas reseñas", "NodeJS (NestJS)");

            apiRest.Uses(reviewController, "");
            reviewController.Uses(reviewRepository,"reads from");
            reviewController.Uses(addReviewComponent, "");
            addReviewComponent.Uses(reviewRepository, "writes");
            reviewRepository.Uses(database,"");

            //Tags
            reviewController.AddTags(componentTag);
            reviewRepository.AddTags(componentTag);
            addReviewComponent.AddTags(componentTag);

            ComponentView componentViewF = viewSet.CreateComponentView(reviewContext, "Review Components", "Components Diagram");
            componentViewF.PaperSize = PaperSize.A4_Landscape;
            componentViewF.AddAllComponents();
            componentViewF.Add(apiRest);
            componentViewF.Add(database);

            structurizrClient.UnlockWorkspace(workspaceId);
            structurizrClient.PutWorkspace(workspaceId, workspace);
        }
    }
}
