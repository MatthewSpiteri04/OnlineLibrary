var OnlineLibrary = angular.module('OnlineLibrary', ['ngRoute']);

OnlineLibrary.config(['$routeProvider', function($routeProvider) {
    $routeProvider
    .when('/home', {
        templateUrl:'views/home.html',
        controller: 'home-controller'
    })
    .when('/login', {
        templateUrl:'views/login.html',
        controller: 'users-controller'
    })
    .when('/sign-up', {
        templateUrl:'views/sign-up.html',
        controller: 'users-controller'
    })
    .when('/help', {
        templateUrl:'views/help.html',
        controller: 'help-controller'
    })
    .when('/helpAnswer/:id', {
        templateUrl:'views/helpAnswer.html',
        controller: 'helpAnswerController'
    })
    .when('/my-info', {
        templateUrl:'views/my-info.html',
        controller: 'myInfo-controller'
    })
    .otherwise({
        redirectTo: '/home'
    });
}]);

OnlineLibrary.service('userService', function($rootScope) {
    var user = null;

    this.getCurrentUser = function() {
        return user;
    };

    this.setMyVariable = function(newValue) {
        user = newValue;
        $rootScope.$broadcast('dataChanged', newValue);
    };
  });


  OnlineLibrary.controller('home-controller', ['$scope', '$http', 'userService', function($scope, $http, userService){
    $scope.user = null;
    $scope.filterOn = false

    $scope.$on('dataChanged', function(event, data) {
        $scope.user = data;
    });


  }]);


  OnlineLibrary.controller('help-controller', ['$scope', '$http', 'userService', function($scope, $http, userService){

    $http.get('https://localhost:44311/api/help')
        .then(response => {
            $scope.helpDetails = response.data;
            console.log($scope.helpDetails);
        })

    $scope.searching = function(search){
    console.log(search);
        $http.get('https://localhost:44311/api/help/' + search)
        .then(response => {
            $scope.helpDetails = response.data;
            console.log($scope.helpDetails);
        })  
    }
  }]);


  /*----------------------------------------*/
  OnlineLibrary.controller('helpAnswerController', ['$scope', '$http', '$routeParams', function($scope, $http, $routeParams) {
    $scope.questionId = $routeParams.id;

    $http.get('https://localhost:44311/api/help/answer/' + $scope.questionId)
        .then(response => {
            $scope.helpDetails = response.data;
            console.log($scope.helpDetails);
        })
    
  }]);
  /*---------------------------------------*/

  OnlineLibrary.controller('myInfo-controller', ['$scope', '$http', 'userService', function($scope, $http, userService){
    $scope.user = userService.getCurrentUser();

    $scope.$on('dataChanged', function(event, data) {
        $scope.user = data;
        console.log($scope.user);
    });


  }]);

// USERS CONTROLLER
OnlineLibrary.controller('users-controller', ['$scope', '$http', 'userService', function($scope, $http, userService){
    $scope.currentUser = {};

    $scope.getRoles = function(id) {
        $http.get('https://localhost:44311/api/User/Roles/' + id)
            .then(response => {
                $scope.currentUser.Roles = response.data;
            })
    }

    $scope.doesUserExist = function(loginForm) {
        $scope.userFound = false;
        $http.get('https://localhost:44311/api/User/Exists/'+ loginForm.login)
            .then(response => {
                $scope.login(response.data, loginForm);
            })
    };

    $scope.login = function(userFound, loginForm) {
        if (userFound) {
            $http.post('https://localhost:44311/api/User/Login', {login: loginForm.login, password: loginForm.password})
            .then(response => {
                if (response.status == 200) {
                    $scope.currentUser = response.data;
                    $scope.getRoles(response.data.id);
                    userService.setMyVariable($scope.currentUser);
                    window.location.href = "#!/home";
                }
                else {
                    console.log('NOT FOUND');
                }
                
            });
        }
        else {
            console.log('NOT FOUND');
        };
    };

    $scope.signUpUser = function(signUpForm) {
        request = {
            firstName: signUpForm.firstName,
            lastName: signUpForm.lastName,
            email: signUpForm.email,
            username: signUpForm.username,
            password: signUpForm.password,
            passwordConfirmation: signUpForm.passwordConfirmation
        }

        if (request.password == request.passwordConfirmation) {
            $http.post('https://localhost:44311/api/User/Add', request)
            .then(response => {
                if (response.status == 200) {
                    $scope.currentUser = response.data;
                    $scope.getRoles(response.data.id);
                    console.log($scope.currentUser);
                    window.location.href = "#!/home";
                }
                else {
                    console.log('FAILED TO CREATE USER');
                }
            });
        }
        else {
            console.log("PASSWORDS DON'T MATCH");
        }
    };

   
    $scope.togglePassword = function() {
        $scope.typePassword = !$scope.typePassword;
        
      };

    $scope.toggleConfirmPassword = function() {
        $scope.typeConfirmPassword = !$scope.typeConfirmPassword;
       
      };

  
}]);

OnlineLibrary.controller('elements-controller', ['$scope', 'userService', function($scope, userService){
    $scope.user = userService.getCurrentUser();

    $scope.$on('dataChanged', function(event, data) {
        $scope.user = data;
        console.log($scope.user);
    });
}]);