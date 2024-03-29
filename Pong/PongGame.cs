using System;
using System.Windows.Forms;
using SharpDX;
using VerySeriousEngine.Core;
using VerySeriousEngine.Input;
using VerySeriousEngine.Objects;

namespace Pong
{
    class PongGame
    {
        static void Main(string[] args)
        {
            var gameWidth = 800;
            var gameHeight = 600;

            var pong = Game.CreateGame("Pong", gameWidth, gameHeight, true);
            var world = new World("Pong World");
            pong.CurrentWorld = world;

            SetupInput(pong.InputManager);

            var camera = new SimpleControllableCamera(objectName: "Camera")
            {
                Location = Vector3.BackwardRH * 100,
            };
            camera.CameraComponent.IsPerspective = false;
            camera.CameraComponent.OrthoWidth = gameWidth;
            camera.CameraComponent.OrthoHeight = gameHeight;

            var gameField = new GameField(gameWidth, gameHeight);
            var gameState = new GameState(gameField)
            {
                BallSpeed = 750.0f,
            };

            var playerController = new PongPlayerController(gameField, new Vector3(350, 0, 0))
            {
                ExitAction = "Exit",
                UpAxis = "Up",
            };
            var compController = new PontAIController(gameField, gameState.Ball, new Vector3(-350, 0, 0));

            pong.StartGame();
            pong.Dispose();
            Console.WriteLine();
            Console.WriteLine("Game finished. Press Enter.");
            Console.ReadLine();
        }

        private static void SetupInput(InputManager inputManager)
        {
            var exitAction = new KeyboardInput(Keys.Escape);
            inputManager.AddAction(exitAction, "Exit");

            var up = new KeyboardInput(Keys.Up);
            var down = new KeyboardInput(Keys.Down, -1);
            inputManager.AddAxis(up, "Up");
            inputManager.AddAxis(down, "Up");
        }
    }
}
