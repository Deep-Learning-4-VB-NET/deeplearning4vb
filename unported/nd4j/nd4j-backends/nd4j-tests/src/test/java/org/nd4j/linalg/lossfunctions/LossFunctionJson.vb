Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports LossBinaryXENT = org.nd4j.linalg.lossfunctions.impl.LossBinaryXENT
Imports LossCosineProximity = org.nd4j.linalg.lossfunctions.impl.LossCosineProximity
Imports LossHinge = org.nd4j.linalg.lossfunctions.impl.LossHinge
Imports LossKLD = org.nd4j.linalg.lossfunctions.impl.LossKLD
Imports LossL1 = org.nd4j.linalg.lossfunctions.impl.LossL1
Imports LossL2 = org.nd4j.linalg.lossfunctions.impl.LossL2
Imports LossMAE = org.nd4j.linalg.lossfunctions.impl.LossMAE
Imports LossMAPE = org.nd4j.linalg.lossfunctions.impl.LossMAPE
Imports LossMCXENT = org.nd4j.linalg.lossfunctions.impl.LossMCXENT
Imports LossMSE = org.nd4j.linalg.lossfunctions.impl.LossMSE
Imports LossMSLE = org.nd4j.linalg.lossfunctions.impl.LossMSLE
Imports LossMultiLabel = org.nd4j.linalg.lossfunctions.impl.LossMultiLabel
Imports LossNegativeLogLikelihood = org.nd4j.linalg.lossfunctions.impl.LossNegativeLogLikelihood
Imports LossPoisson = org.nd4j.linalg.lossfunctions.impl.LossPoisson
Imports LossSquaredHinge = org.nd4j.linalg.lossfunctions.impl.LossSquaredHinge
Imports DeserializationFeature = org.nd4j.shade.jackson.databind.DeserializationFeature
Imports MapperFeature = org.nd4j.shade.jackson.databind.MapperFeature
Imports ObjectMapper = org.nd4j.shade.jackson.databind.ObjectMapper
Imports SerializationFeature = org.nd4j.shade.jackson.databind.SerializationFeature
import static org.junit.jupiter.api.Assertions.assertEquals

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

Namespace org.nd4j.linalg.lossfunctions

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.TRAINING) @NativeTag @Tag(TagNames.LOSS_FUNCTIONS) @Tag(TagNames.JACKSON_SERDE) public class LossFunctionJson extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class LossFunctionJson
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testJsonSerialization(org.nd4j.linalg.factory.Nd4jBackend backend) throws Exception
'JAVA TO VB CONVERTER WARNING: Method 'throws' clauses are not available in VB:
		Public Overridable Sub testJsonSerialization(ByVal backend As Nd4jBackend)

			Dim w As INDArray = Nd4j.create(New Double() {1.0, 2.0, 3.0})

			Dim lossFns() As ILossFunction = {
				New LossBinaryXENT(),
				New LossBinaryXENT(w),
				New LossCosineProximity(),
				New LossHinge(),
				New LossKLD(),
				New LossL1(),
				New LossL1(w),
				New LossL2(),
				New LossL2(w),
				New LossMAE(),
				New LossMAE(w),
				New LossMAPE(),
				New LossMAPE(w),
				New LossMCXENT(),
				New LossMCXENT(w),
				New LossMSE(),
				New LossMSE(w),
				New LossMSLE(),
				New LossMSLE(w),
				New LossNegativeLogLikelihood(),
				New LossNegativeLogLikelihood(w),
				New LossPoisson(),
				New LossSquaredHinge(),
				New LossMultiLabel()
			}

			Dim mapper As New ObjectMapper()
			mapper.configure(DeserializationFeature.FAIL_ON_UNKNOWN_PROPERTIES, False)
			mapper.configure(SerializationFeature.FAIL_ON_EMPTY_BEANS, False)
			mapper.configure(MapperFeature.SORT_PROPERTIES_ALPHABETICALLY, True)
			mapper.enable(SerializationFeature.INDENT_OUTPUT)

			For Each lf As ILossFunction In lossFns
				Dim asJson As String = mapper.writeValueAsString(lf)
				'            System.out.println(asJson);

				Dim fromJson As ILossFunction = mapper.readValue(asJson, GetType(ILossFunction))
				assertEquals(lf, fromJson)
			Next lf
		End Sub


		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace