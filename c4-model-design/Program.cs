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
            const long workspaceId = 78274; //Cambiar el workspaceID de acuerdo a su workspace
            const string apiKey = "4e2b098f-e00d-42ea-8ad2-8481623f5f66"; // cambiar el apiKey de acuerdo a su workspace 
            const string apiSecret = "be9a8c8f-33e4-4cf1-ab6e-ed62eecba623"; // cambiar el apiSecret de Acuerdo a su workspace

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
            Container database = docSeekerSystem.AddContainer("Database", "", "Oracle");

            //Modulos
            Container reviewContext = docSeekerSystem.AddContainer("Review Context", "Bounded Context of Real-time monitoring of the payment process and notifications.", "NodeJS (NestJS)");
            Container registerContext = docSeekerSystem.AddContainer("Register Context", "Bounded Context de creación de una nueva cuenta de usuario", "NodeJS (NestJS)");
            Container logInContext = docSeekerSystem.AddContainer("Log in Context", "Bounded Context de inicio de sesión por parte del usuario", "NodeJS (NestJS)");
            Container notificationContext = docSeekerSystem.AddContainer("Notification Context", "Bounded Context de Notificaciones", "NodeJS (NestJS)");
            Container PaymentContext = docSeekerSystem.AddContainer("Payment Context", "Payment Context de pagos", "NodeJS (NestJS)");
            Container AppointmentContext = docSeekerSystem.AddContainer("Appointment Context", "Appointment Context de citas", "NodeJS (NestJS)");
            Container PrescriptionContext = docSeekerSystem.AddContainer("Prescription Context", "Prescription Context", "NodeJS (NestJS)");

            patient.Uses(mobileApplication, "Query");
            doctor.Uses(mobileApplication, "Query");

            mobileApplication.Uses(apiRest, "API Request", "JSON/HTTPS");

            apiRest.Uses(reviewContext, "", "");
            apiRest.Uses(registerContext, "", "");
            apiRest.Uses(logInContext, "", "");
            apiRest.Uses(notificationContext, "", "");
            apiRest.Uses(PaymentContext, "", "");
            apiRest.Uses(AppointmentContext, "", "");
            apiRest.Uses(PrescriptionContext, "", "");

            reviewContext.Uses(database, "", "");
            registerContext.Uses(database, "", "");
            logInContext.Uses(database, "", "");
            notificationContext.Uses(database, "", "");
            PaymentContext.Uses(database, "", "");
            AppointmentContext.Uses(database, "", "");
            PrescriptionContext.Uses(database, "", "");

            PaymentContext.Uses(paymentGatewaySystem, "API Request", "JSON/HTTPS");
            notificationContext.Uses(emailSystem, "API Request", "JSON/HTTPS");
            registerContext.Uses(emailSystem, "API Request", "JSON/HTTPS");

            // Tags
            mobileApplication.AddTags("MobileApp");
            apiRest.AddTags("APIRest");
            database.AddTags("Database");

            string contextTag = "Context";

            reviewContext.AddTags(contextTag);
            registerContext.AddTags(contextTag);
            logInContext.AddTags(contextTag);
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

            // 3. Diagrama de componentes

            string componentTag = "component";
            styles.Add(new ElementStyle("component") { Shape = Shape.Component, Background = "#facc2e", Icon = "" });



            // 3.1. Registration context

            Component registerController = registerContext.AddComponent("Register Controller", "Controla el registro de nuevos usuarios.", "NodeJS (NestJS)");
            Component registerVerification = registerContext.AddComponent("Register Verification", "Verifica la información ingresada por el usuario es correcta.", "NodeJS (NestJS)");
            Component registerRepository = registerContext.AddComponent("Register Repository", "Información sobre el nuevo usuario.", "NodeJS (NestJS)");
            Component emailConfirmation = registerContext.AddComponent("Email Confirmation", "Envía email de confirmación tras haber creado una nueva cuenta.", "NodeJS (NestJS)");

            apiRest.Uses(registerController, "API calls");

            registerController.Uses(registerVerification, "uses");
            registerController.Uses(emailConfirmation, "uses");

            registerVerification.Uses(emailSystem, "uses");
            registerVerification.Uses(registerRepository, "uses");

            emailConfirmation.Uses(emailSystem, "uses");

            registerRepository.Uses(database, "uses");

            //Tags

            registerController.AddTags(componentTag);
            registerVerification.AddTags(componentTag);
            registerRepository.AddTags(componentTag);
            emailConfirmation.AddTags(componentTag);


            ComponentView componentView3_1 = viewSet.CreateComponentView(registerContext, "Registration Component", "Component Diagrams");
            componentView3_1.PaperSize = PaperSize.A4_Landscape;
            componentView3_1.AddAllComponents();
            componentView3_1.Add(apiRest);
            componentView3_1.Add(database);
            componentView3_1.Add(emailSystem);

            // 3.2 Log in Context

            Component signInController = logInContext.AddComponent("Sign In Controller", "Permite al usuario acceder a su cuenta personal.", "NodeJS (NestJS)");
            Component userVerifyComponent = logInContext.AddComponent("Users Repository", "Información sobre el usuario.", "NodeJS (NestJS)");

            apiRest.Uses(signInController, "API calls");
            signInController.Uses(userVerifyComponent, "reads from");
            userVerifyComponent.Uses(database, "");

            //Tags

            signInController.AddTags(componentTag);
            userVerifyComponent.AddTags(componentTag);


            ComponentView componentView = viewSet.CreateComponentView(logInContext, "Security Component", "Component Diagrams");
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
            prescriptionController.Uses(diagnosisRepository, "reads and writes");
            prescriptionController.Uses(medicineRepository, "reads and writes");
            prescriptionController.Uses(therapyRepository, "reads and writes");
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
            reviewController.Uses(reviewRepository, "reads from");
            reviewController.Uses(addReviewComponent, "");
            addReviewComponent.Uses(reviewRepository, "writes");
            reviewRepository.Uses(database, "");

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