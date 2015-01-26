/************************************************************************************

Copyright   :   Copyright 2014 Oculus VR, LLC. All Rights reserved.

Licensed under the Oculus VR Rift SDK License Version 3.2 (the "License");
you may not use the Oculus VR Rift SDK except in compliance with the License,
which is provided at the time of installation or download, or which
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculusvr.com/licenses/LICENSE-3.2

Unless required by applicable law or agreed to in writing, the Oculus VR SDK
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

Shader "Nature/Terrain/Diffuse"
{
    Properties
    {
        [HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
        [HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
        [HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
        [HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
        [HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
        //Used in fallback on old cards & base map
        [HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
        [HideInInspector] _Color ("Main Color", Color) = (1,1,1,1)
    }
 
    SubShader
    {
        Tags
        {
            "SplatCount" = "4"
            "Queue" = "Geometry-100"
            "RenderType" = "Opaque"
        }
        Blend SrcAlpha OneMinusSrcAlpha
        CGPROGRAM
        #pragma surface surf Lambert
        struct Input
        {
            float2 uv_Control : TEXCOORD0;
            float2 uv_Splat0 : TEXCOORD1;
            float2 uv_Splat1 : TEXCOORD2;
            float2 uv_Splat2 : TEXCOORD3;
            float2 uv_Splat3 : TEXCOORD4;
        };
 
        sampler2D _Control;
        sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
 
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 splat_control = tex2D (_Control, IN.uv_Control);
            fixed4 firstSplat = tex2D (_Splat0, IN.uv_Splat0);
            fixed3 col;
            col = splat_control.r * tex2D (_Splat0, IN.uv_Splat0).rgb;
            col += splat_control.g * tex2D (_Splat1, IN.uv_Splat1).rgb;
            col += splat_control.b * tex2D (_Splat2, IN.uv_Splat2).rgb;
            col += splat_control.a * tex2D (_Splat3, IN.uv_Splat3).rgb;
            o.Albedo = col;
            o.Alpha = 1;
            if(tex2D(_Splat0, IN.uv_Splat0).a == 0)
                o.Alpha = 1 - splat_control.r;
            else if(tex2D(_Splat1, IN.uv_Splat1).a == 0)
                o.Alpha = 1 - splat_control.g;
            else if(tex2D(_Splat2, IN.uv_Splat2).a == 0)
                o.Alpha = 1 - splat_control.b;
            else if(tex2D(_Splat3, IN.uv_Splat3).a == 0)
                o.Alpha = 1 - splat_control.a;
        }
        ENDCG
    }
 
    Dependency "AddPassShader" = "Hidden/TerrainEngine/Splatmap/Lightmap-AddPass"
    Dependency "BaseMapShader" = "Diffuse"
 
    //Fallback to Diffuse
    Fallback "Diffuse"
}
