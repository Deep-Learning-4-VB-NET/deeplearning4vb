Imports System
Imports System.Collections.Generic
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports DifferentialFunction = org.nd4j.autodiff.functions.DifferentialFunction
Imports NoOpNameFoundException = org.nd4j.imports.NoOpNameFoundException

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

Namespace org.nd4j.imports.converters


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j public class ImportClassMapping
	Public Class ImportClassMapping

		Private Shared ReadOnly OP_NAME_MAP As IDictionary(Of String, DifferentialFunction) = New Dictionary(Of String, DifferentialFunction)()
		Private Shared ReadOnly TF_OP_NAME_MAP As IDictionary(Of String, DifferentialFunction) = New Dictionary(Of String, DifferentialFunction)()
		Private Shared ReadOnly ONNX_OP_NAME_MAP As IDictionary(Of String, DifferentialFunction) = New Dictionary(Of String, DifferentialFunction)()

		Private Shared ReadOnly fnClasses As IList(Of Type) = New List(Of Type) From {Of Type}

		Shared Sub New()
			For Each c As Type In fnClasses
				Try
					Dim df As DifferentialFunction = CType(System.Activator.CreateInstance(c), DifferentialFunction)

					Dim opName As String = df.opName()
					OP_NAME_MAP(opName) = df

					'TF import mapping
					Try
						Dim tfNames() As String = df.tensorflowNames()
						For Each s As String In tfNames
							If TF_OP_NAME_MAP.ContainsKey(s) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
								log.warn("Duplicate TF op mapping found for op {}: {} vs {}", s, TF_OP_NAME_MAP(s).GetType().FullName, df.GetType().FullName)
							End If
							TF_OP_NAME_MAP(s) = df
						Next s
					Catch e As NoOpNameFoundException
						'Ignore
					End Try

					'ONNX import mapping
					Try
						Dim tfNames() As String = df.onnxNames()
						For Each s As String In tfNames
							If ONNX_OP_NAME_MAP.ContainsKey(s) Then
'JAVA TO VB CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
								log.warn("Duplicate ONNX op mapping found for op {}: {} vs {}", s, ONNX_OP_NAME_MAP(s).GetType().FullName, df.GetType().FullName)
							End If
							ONNX_OP_NAME_MAP(s) = df
						Next s
					Catch e As NoOpNameFoundException
						'Ignore
					End Try

				Catch t As Exception
					Throw New Exception(t)
				End Try
			Next c
		End Sub

		Public Shared ReadOnly Property OpClasses As IList(Of [Class])
			Get
				Return fnClasses
			End Get
		End Property

		Public Shared ReadOnly Property TFOpMappingFunctions As IDictionary(Of String, DifferentialFunction)
			Get
				Return TF_OP_NAME_MAP
			End Get
		End Property

		Public Shared ReadOnly Property OnnxOpMappingFunctions As IDictionary(Of String, DifferentialFunction)
			Get
				Return ONNX_OP_NAME_MAP
			End Get
		End Property

		Public Shared ReadOnly Property OpNameMapping As IDictionary(Of String, DifferentialFunction)
			Get
				Return OP_NAME_MAP
			End Get
		End Property

	End Class

End Namespace