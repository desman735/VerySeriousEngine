﻿using SharpDX.Direct3D11;
using System;
using VerySeriousEngine.Core;
using VerySeriousEngine.Geometry;
using VerySeriousEngine.Utils.Import;

using Buffer = SharpDX.Direct3D11.Buffer;

namespace VerySeriousEngine.Shaders
{
    abstract public class Shader : IDisposable
    {
        public InputLayout InputLayout { get; private set; }
        public VertexShader VertexShader { get; private set; }
        public PixelShader PixelShader { get; private set; }

        public Shader(string shaderFileName, InputElement[] inputElements, string vertexShaderEntryPoint, string pixelShaderEntryPoint)
        {
            var constructor = Game.GameInstance.GameConstructor;
            var vertexShaderCompileResult = constructor.CompileVertexShader(shaderFileName, vertexShaderEntryPoint, inputElements);
            VertexShader = vertexShaderCompileResult.Item1;
            InputLayout = vertexShaderCompileResult.Item2;
            PixelShader = constructor.CompilePixelShader(shaderFileName, pixelShaderEntryPoint);
        }

        abstract public void PrepareResources(Renderer renderer);

        virtual public void Dispose()
        {
            InputLayout.Dispose();
            VertexShader.Dispose();
            PixelShader.Dispose();
        }
    }

    public class VertexColorShader : Shader
    {
        public VertexColorShader() : base("Shaders/Code/VertexColorShader.hlsl", Vertex.InputElements, "VSMain", "PSMain")
        { }

        public override void PrepareResources(Renderer renderer)
        { }
    }

    public class TextureShader : Shader
    {
        private readonly ShaderResourceView textureResource;

        public TextureShader(string texturePath) : base("Shaders/Code/TextureShader.hlsl", Vertex.InputElements, "VSMain", "PSMain")
        {
            textureResource = TextureImporter.ImportTextureFromFile(texturePath);
        }

        public override void Dispose()
        {
            textureResource.Dispose();
            base.Dispose();
        }

        public override void PrepareResources(Renderer renderer)
        {
            renderer.Context.PixelShader.SetShaderResource(0, textureResource);
        }
    }

    public class PhongShader : Shader
    {
        private Buffer constantBuffer;
        private ShaderResourceView textureResource;

        private readonly float[] constants;

        public float SpecularReflection {
            get => constants[0];
            set {
                if (constants[0] == value)
                    return;

                constants[0] = value;
                UpdateConstantBuffer();
            }
        }
        public float DiffuseReflection {
            get => constants[1];
            set {
                if (constants[1] == value)
                    return;

                constants[1] = value;
                UpdateConstantBuffer();
            }
        }
        public float AmbientReflection {
            get => constants[2];
            set {
                if (constants[2] == value)
                    return;

                constants[2] = value;
                UpdateConstantBuffer();
            }
        }
        public float Shininess {
            get => constants[3];
            set {
                if (constants[3] == value)
                    return;

                constants[3] = value;
                UpdateConstantBuffer();
            }
        }

        public PhongShader(string texturePath) : base("Shaders/Code/PhongTextureShader.hlsl", Vertex.InputElements, "VSMain", "PSMain")
        {
            constants = new float[4] { .5f, .5f, .5f, .5f };

            constantBuffer = Game.GameInstance.GameConstructor.CreateBuffer(constants, BindFlags.ConstantBuffer);
            textureResource = TextureImporter.ImportTextureFromFile(texturePath);
        }

        public override void Dispose()
        {
            constantBuffer.Dispose();
            textureResource.Dispose();
            base.Dispose();
        }

        public override void PrepareResources(Renderer renderer)
        {
            renderer.Context.PixelShader.SetShaderResource(0, textureResource);
            renderer.Context.PixelShader.SetConstantBuffer(1, constantBuffer);
        }

        private void UpdateConstantBuffer()
        {
            Game.GameInstance.GameRenderer.Context.UpdateSubresource(constants, constantBuffer);
        }
    }
}
