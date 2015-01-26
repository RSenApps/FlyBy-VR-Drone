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

Shader "DoubleSided" {
    Properties {

    _Color ("Main Color", Color) = (1,1,1,1)

    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}

}

 

SubShader {

    Blend SrcAlpha OneMinusSrcAlpha
    
    Cull Off

   

    CGPROGRAM

    #pragma surface surf Lambert

   

    sampler2D _MainTex;

    fixed4 _Color;

   

    struct Input {

        float2 uv_MainTex;

    };

   

    void surf (Input IN, inout SurfaceOutput o) {

        fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

        o.Albedo = c.rgb;

        o.Alpha =  c.a;

    }

    ENDCG

    }

 

Fallback "Diffuse"
}
