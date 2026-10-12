Imports System
Imports System.Collections.Generic
Imports Microsoft.VisualBasic
Imports BeforeEach = org.junit.jupiter.api.BeforeEach
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports ActivationCube = org.nd4j.linalg.activations.impl.ActivationCube
Imports ActivationELU = org.nd4j.linalg.activations.impl.ActivationELU
Imports ActivationGELU = org.nd4j.linalg.activations.impl.ActivationGELU
Imports ActivationHardSigmoid = org.nd4j.linalg.activations.impl.ActivationHardSigmoid
Imports ActivationHardTanH = org.nd4j.linalg.activations.impl.ActivationHardTanH
Imports ActivationIdentity = org.nd4j.linalg.activations.impl.ActivationIdentity
Imports ActivationLReLU = org.nd4j.linalg.activations.impl.ActivationLReLU
Imports ActivationRReLU = org.nd4j.linalg.activations.impl.ActivationRReLU
Imports ActivationRationalTanh = org.nd4j.linalg.activations.impl.ActivationRationalTanh
Imports ActivationReLU = org.nd4j.linalg.activations.impl.ActivationReLU
Imports ActivationSigmoid = org.nd4j.linalg.activations.impl.ActivationSigmoid
Imports ActivationSoftPlus = org.nd4j.linalg.activations.impl.ActivationSoftPlus
Imports ActivationSoftSign = org.nd4j.linalg.activations.impl.ActivationSoftSign
Imports ActivationSoftmax = org.nd4j.linalg.activations.impl.ActivationSoftmax
Imports ActivationTanH = org.nd4j.linalg.activations.impl.ActivationTanH
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports org.nd4j.common.primitives
Imports org.nd4j.shade.jackson.databind
import static org.junit.jupiter.api.Assertions.assertEquals
import static org.junit.jupiter.api.Assertions.assertTrue

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

Namespace org.nd4j.linalg.activations




'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NativeTag public class TestActivation extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class TestActivation
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

		Private mapper As ObjectMapper

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @BeforeEach public void initMapper()
		Public Overridable Sub initMapper()
			mapper = New ObjectMapper()
			mapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, False)
			mapper.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, False)
			mapper.configure(MapperFeature.SORT_PROPERTIES_ALPHABETICALLY, True)
			mapper.enable(SerializationFeature.INDENT_OUTPUT)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testRelu(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testRelu(ByVal backend As Nd4jBackend)

			Dim max() As Double? = {Nothing, 6.0, 2.5, 5.0}
			Dim threshold() As Double? = {0.0, 0.0, 0.75, 0.2}
			Dim negativeSlope() As Double? = {0.0, 0.0, 0.0, 0.3}

			Dim [in] As INDArray = Nd4j.linspace(-10, 10, 1000, DataType.DOUBLE)
			Dim dIn() As Double = [in].data().asDouble()

			For i As Integer = 0 To max.Length - 1
				Dim r As New ActivationReLU(max(i), threshold(i), negativeSlope(i))
				Dim [out] As INDArray = r.getActivation([in].dup(), True)
				Dim exp(dIn.Length - 1) As Double
				For j As Integer = 0 To exp.Length - 1
					If max(i) IsNot Nothing AndAlso dIn(j) >= max(i) Then
						exp(j) = max(i)
					ElseIf dIn(j) < threshold(i) Then
						exp(j) = negativeSlope(i).Value * (dIn(j) - threshold(i).Value)
					Else
						exp(j) = Math.Min(dIn(j),If(max(i) Is Nothing, Double.MaxValue, max(i)))
					End If
				Next j
				Dim expArr As INDArray = Nd4j.createFromArray(exp)
				assertEquals(expArr, [out])
			Next i

			'Test backprop
			Dim eps As INDArray = Nd4j.arange([in].length()).castTo(DataType.DOUBLE)
			Dim dEps() As Double = eps.data().asDouble()
			For i As Integer = 0 To max.Length - 1
				Dim r As New ActivationReLU(max(i), threshold(i), negativeSlope(i))
				Dim p As Pair(Of INDArray, INDArray) = r.backprop([in].dup(), eps.dup())
				Dim grad As INDArray = p.First
				Dim dGrad() As Double = grad.data().asDouble()

				For j As Integer = 0 To dGrad.Length - 1
					If max(i) IsNot Nothing AndAlso dIn(j) >= max(i) Then
						'Max segment - gradient at input should be zero
						assertEquals(0.0, dGrad(j), 0.0)
					ElseIf dIn(j) < threshold(i) Then
						'Below threshold - gradient equal to dL/dOut * threshold
						Dim exp As Double = dEps(j) * negativeSlope(i).Value
						assertEquals(exp, dGrad(j), 1e-6)
					Else
						'Linear part
						assertEquals(dEps(j), dGrad(j), 1e-8)
					End If
				Next j
			Next i
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") @Tag(org.nd4j.common.tests.tags.TagNames.JACKSON_SERDE) public void testJson(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testJson(ByVal backend As Nd4jBackend)

			Dim activations() As IActivation = {
				New ActivationCube(),
				New ActivationELU(0.25),
				New ActivationHardSigmoid(),
				New ActivationHardTanH(),
				New ActivationIdentity(),
				New ActivationLReLU(0.25),
				New ActivationRationalTanh(),
				New ActivationReLU(),
				New ActivationRReLU(0.25, 0.5),
				New ActivationSigmoid(),
				New ActivationSoftmax(),
				New ActivationSoftPlus(),
				New ActivationSoftSign(),
				New ActivationTanH(),
				New ActivationGELU(),
				New ActivationGELU(True)
			}

			Dim expectedFields()() As String = {
				New String() {"@class"},
				New String() {"@class", "alpha"},
				New String() {"@class"},
				New String() {"@class"},
				New String() {"@class"},
				New String() {"@class", "alpha"},
				New String() {"@class"},
				New String() {"@class", "max", "negativeSlope", "threshold"},
				New String() {"@class", "l", "u"},
				New String() {"@class"},
				New String() {"@class"},
				New String() {"@class"},
				New String() {"@class"},
				New String() {"@class"},
				New String() {"@class", "precise"},
				New String() {"@class", "precise"}
			}

			For i As Integer = 0 To activations.Length - 1
				Dim asJson As String = mapper.writeValueAsString(activations(i))

				Dim node As JsonNode = mapper.readTree(asJson)

				Dim fieldNamesIter As IEnumerator(Of String) = node.fieldNames()
				Dim actualFieldsByName As IList(Of String) = New List(Of String)()
				Do While fieldNamesIter.MoveNext()
					actualFieldsByName.Add(fieldNamesIter.Current)
				Loop

				Dim expFields() As String = expectedFields(i)

				Dim msg As String = activations(i).ToString() & vbTab & "Expected fields: " & Arrays.toString(expFields) & vbTab & "Actual fields: " & actualFieldsByName
				assertEquals(expFields.Length, actualFieldsByName.Count,msg)

				For Each s As String In expFields
					msg = "Expected field """ & s & """, was not found in " & activations(i).ToString()
					assertTrue(actualFieldsByName.Contains(s),msg)
				Next s

				'Test conversion from JSON:
				Dim act As IActivation = mapper.readValue(asJson, GetType(IActivation))
				assertEquals(activations(i), act)
			Next i
		End Sub
	End Class

End Namespace