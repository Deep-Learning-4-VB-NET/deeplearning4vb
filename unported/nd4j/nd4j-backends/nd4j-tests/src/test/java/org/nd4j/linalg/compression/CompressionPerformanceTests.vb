Imports System.IO
Imports Slf4j = lombok.extern.slf4j.Slf4j
Imports val = lombok.val
Imports Disabled = org.junit.jupiter.api.Disabled
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports NativeTag = org.nd4j.common.tests.tags.NativeTag
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend
Imports SerializationUtils = org.nd4j.common.util.SerializationUtils

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

Namespace org.nd4j.linalg.compression


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Slf4j @Disabled @NativeTag @Tag(TagNames.COMPRESSION) public class CompressionPerformanceTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class CompressionPerformanceTests
		Inherits BaseNd4jTestWithBackends

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void groundTruthTests_Threshold_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub groundTruthTests_Threshold_1(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(119)
			Dim params As val = Nd4j.rand(New Long(){1, 50000000}, -1.0, 1.0, Nd4j.Random)
			Dim original As val = params.dup(params.ordering())

			Dim iterations As Integer = 1000

			Dim timeE As Long = 0
			Dim timeS As Long = 0
			Dim timeD As Long = 0
			Dim s As Integer = 0
			For e As Integer = 0 To iterations - 1
				Dim timeES As val = DateTimeHelper.CurrentUnixTimeMillis()
				Dim encoded As val = Nd4j.Executioner.thresholdEncode(params, 0.99)
				Dim timeEE As val = DateTimeHelper.CurrentUnixTimeMillis()


				params.assign(original)
				timeE += (timeEE - timeES)


				Dim bs As val = New MemoryStream()
				Dim timeSS As val = DateTimeHelper.CurrentUnixTimeMillis()
				SerializationUtils.serialize(encoded, bs)
				Dim timeSE As val = DateTimeHelper.CurrentUnixTimeMillis()

				timeS += (timeSE - timeSS)

				Dim ba As val = bs.toByteArray()
				Dim timeDS As val = DateTimeHelper.CurrentUnixTimeMillis()
				Dim deserialized As val = SerializationUtils.deserialize(ba)
				Dim timeDE As val = DateTimeHelper.CurrentUnixTimeMillis()
				timeD += (timeDE - timeDS)

				s = bs.size()
			Next e


			log.info("Encoding time: {} ms; Serialization time: {} ms; Deserialized time: {} ms; Serialized bytes: {}", timeE \ iterations, timeS \ iterations, timeD \ iterations, s)
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void groundTruthTests_Bitmap_1(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub groundTruthTests_Bitmap_1(ByVal backend As Nd4jBackend)
			Nd4j.Random.setSeed(119)
			Dim params As val = Nd4j.rand(New Long(){1, 25000000}, -1.0, 1.0, Nd4j.Random)
			Dim original As val = params.dup(params.ordering())

			Dim iterations As Integer = 1000

			Dim buffer As DataBuffer = Nd4j.DataBufferFactory.createInt(params.length() \ 16 + 5)

			Dim ret As INDArray = Nd4j.createArrayFromShapeBuffer(buffer, params.shapeInfoDataBuffer())

			Dim time As Long = 0
			For e As Integer = 0 To iterations - 1
				Dim timeES As val = DateTimeHelper.CurrentUnixTimeMillis()
				Nd4j.Executioner.bitmapEncode(params, ret,0.99)
				Dim timeEE As val = DateTimeHelper.CurrentUnixTimeMillis()


				params.assign(original)
				Nd4j.MemoryManager.memset(ret)
				time += (timeEE - timeES)
			Next e


			log.info("Encoding time: {} ms;", time \ iterations)
		End Sub

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function
	End Class

End Namespace