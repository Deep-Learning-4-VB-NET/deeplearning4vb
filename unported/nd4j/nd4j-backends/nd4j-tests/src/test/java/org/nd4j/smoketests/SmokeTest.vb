Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports ProfilerConfig = org.nd4j.linalg.profiler.ProfilerConfig

'
' *
' *  *  ******************************************************************************
' *  *  *
' *  *  *
' *  *  * This program and the accompanying materials are made available under the
' *  *  * terms of the Apache License, Version 2.0 which is available at
' *  *  * https://www.apache.org/licenses/LICENSE-2.0.
' *  *  *
' *  *  *  See the NOTICE file distributed with this work for additional
' *  *  *  information regarding copyright ownership.
' *  *  * Unless required by applicable law or agreed to in writing, software
' *  *  * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
' *  *  * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
' *  *  * License for the specific language governing permissions and limitations
' *  *  * under the License.
' *  *  *
' *  *  * SPDX-License-Identifier: Apache-2.0
' *  *  *****************************************************************************
' *
' *
' 

Namespace org.nd4j.smoketests

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @NativeTag public class SmokeTest
	Public Class SmokeTest
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testBasic()
		Public Overridable Sub testBasic()
			Nd4j.Environment.Debug = True
			Nd4j.Executioner.ProfilingConfig = ProfilerConfig.builder().checkForNAN(True).checkForINF(True).checkLocality(True).checkElapsedTime(True).checkWorkspaces(True).build()
			Dim arr As INDArray = Nd4j.randn(2,2)
			Dim arr2 As INDArray = Nd4j.randn(2,2)
			For Each dataType As DataType In DataType.values()
				If Not dataType.isFPType() Then
					Continue For
				End If
				log.info("Testing matrix multiply on data type {}",dataType)
				Dim casted As INDArray = arr.castTo(dataType)
				Dim casted2 As INDArray = arr2.castTo(dataType)
				Dim result As INDArray = casted.mmul(casted2)
				log.info("Result for data type {} was {}",dataType,result)

			Next dataType
		End Sub

	End Class

End Namespace