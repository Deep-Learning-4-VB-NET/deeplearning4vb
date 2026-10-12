Imports System
Imports System.Collections.Generic
Imports NonNull = lombok.NonNull
Imports ILossFunction = org.nd4j.linalg.lossfunctions.ILossFunction
Imports org.nd4j.linalg.lossfunctions.impl
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper

'
' *  ******************************************************************************
' *  *
' *  *
' *  * This program and the accompanying materials are made available under the
' *  * terms of the Apache License, Version 2.0 which is available at
' *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *
' *  *  See the NOTICE file distributed with this work for additional
' *  *  information regarding copyright ownership.
' *  * Unless required by applicable law or agreed to in writing, software
' *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  * License for the specific language governing permissions and limitations
' *  * under the License.
' *  *
' *  * SPDX-License-Identifier: Apache-2.0
' *  *****************************************************************************
' 

Namespace org.nd4j.serde.json



	Public Class LegacyILossFunctionDeserializer
		Inherits BaseLegacyDeserializer(Of ILossFunction)

		Private Shared ReadOnly LEGACY_NAMES As IDictionary(Of String, String) = New Dictionary(Of String, String)()

		Private Shared legacyMapper As ObjectMapper

		Public Shared Property LegacyJsonMapper As ObjectMapper
			Set(ByVal mapper As ObjectMapper)
				legacyMapper = mapper
			End Set
			Get
				Return legacyMapper
			End Get
		End Property

		Shared Sub New()
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LEGACY_NAMES("BinaryXENT") = GetType(LossBinaryXENT).FullName
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LEGACY_NAMES("CosineProximity") = GetType(LossCosineProximity).FullName
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LEGACY_NAMES("Hinge") = GetType(LossHinge).FullName
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LEGACY_NAMES("KLD") = GetType(LossKLD).FullName
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LEGACY_NAMES("MAE") = GetType(LossMAE).FullName
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LEGACY_NAMES("L1") = GetType(LossL1).FullName
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LEGACY_NAMES("MAPE") = GetType(LossMAPE).FullName
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LEGACY_NAMES("MCXENT") = GetType(LossMCXENT).FullName
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LEGACY_NAMES("MSE") = GetType(LossMSE).FullName
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LEGACY_NAMES("L2") = GetType(LossL2).FullName
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LEGACY_NAMES("MSLE") = GetType(LossMSLE).FullName
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LEGACY_NAMES("NegativeLogLikelihood") = GetType(LossNegativeLogLikelihood).FullName
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LEGACY_NAMES("Poisson") = GetType(LossPoisson).FullName
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LEGACY_NAMES("SquaredHinge") = GetType(LossSquaredHinge).FullName
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LEGACY_NAMES("MultiLabel") = GetType(LossMultiLabel).FullName
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LEGACY_NAMES("FMeasure") = GetType(LossFMeasure).FullName

			'The following didn't previously have subtype annotations - hence will be using default name (class simple name)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LEGACY_NAMES(GetType(LossMixtureDensity).Name) = GetType(LossMixtureDensity).FullName
		End Sub


		Public Overrides ReadOnly Property LegacyNamesMap As IDictionary(Of String, String)
			Get
				Return LEGACY_NAMES
			End Get
		End Property


		Public Overrides ReadOnly Property DeserializedType As Type
			Get
				Return GetType(ILossFunction)
			End Get
		End Property

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void registerLegacyClassDefaultName(@NonNull @Class clazz)
		Public Shared Sub registerLegacyClassDefaultName(ByVal clazz As Type)
			registerLegacyClassSpecifiedName(clazz.Name, clazz)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: public static void registerLegacyClassSpecifiedName(@NonNull String name, @NonNull @Class clazz)
		Public Shared Sub registerLegacyClassSpecifiedName(ByVal name As String, ByVal clazz As Type)
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			LEGACY_NAMES(name) = clazz.FullName
		End Sub
	End Class

End Namespace