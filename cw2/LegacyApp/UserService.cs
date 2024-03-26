using System;

namespace LegacyApp
{
    public class UserService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserCreditService _userCreditService;

        // Parameterless constructor for legacy compatibility
        public UserService() : this(new ClientRepository(), new UserCreditService())
        {
        }

        // Constructor that allows dependency injection for better testability
        public UserService(IClientRepository clientRepository, IUserCreditService userCreditService)
        {
            _clientRepository = clientRepository ?? throw new ArgumentNullException(nameof(clientRepository));
            _userCreditService = userCreditService ?? throw new ArgumentNullException(nameof(userCreditService));
        }

        public bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId)
        {
            if (!IsValidUserInput(firstName, lastName, email, dateOfBirth)) return false;

            var client = _clientRepository.GetById(clientId);
            if (client == null) 
            {
                Console.WriteLine("Client not found.");
                return false;
            }

            var user = CreateUser(firstName, lastName, email, dateOfBirth, client);

            if (user.HasCreditLimit && user.CreditLimit < 500) 
            {
                Console.WriteLine("Credit limit too low.");
                return false;
            }

            UserDataAccess.AddUser(user);
            return true;
        }

        private bool IsValidUserInput(string firstName, string lastName, string email, DateTime dateOfBirth)
        {
            return !(string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || !email.Contains("@"));
        }

        private User CreateUser(string firstName, string lastName, string email, DateTime dateOfBirth, Client client)
        {
            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = email,
                DateOfBirth = dateOfBirth,
                Client = client
            };

            DetermineCreditLimit(user, client);

            return user;
        }

        private void DetermineCreditLimit(User user, Client client)
        {
            if (client.Type == "VeryImportantClient")
            {
                user.HasCreditLimit = false;
            }
            else
            {
                user.HasCreditLimit = true;
                int baseCreditLimit = _userCreditService.GetCreditLimit(user.LastName, user.DateOfBirth);
                user.CreditLimit = (client.Type == "ImportantClient") ? baseCreditLimit * 2 : baseCreditLimit;
            }
        }
    }
}
