import static org.junit.jupiter.api.Assertions.assertTrue
import static org.junit.jupiter.api.Assertions.fail
Imports Tag = org.junit.jupiter.api.Tag
Imports Test = org.junit.jupiter.api.Test
Imports ParameterizedTest = org.junit.jupiter.params.ParameterizedTest
Imports MethodSource = org.junit.jupiter.params.provider.MethodSource
Imports TagNames = org.nd4j.common.tests.tags.TagNames
Imports BaseNd4jTestWithBackends = org.nd4j.linalg.BaseNd4jTestWithBackends
Imports Conv1DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv1DConfig
Imports Conv2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv2DConfig
Imports Conv3DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Conv3DConfig
Imports DeConv2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.DeConv2DConfig
Imports DeConv3DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.DeConv3DConfig
Imports PaddingMode = org.nd4j.linalg.api.ops.impl.layers.convolution.config.PaddingMode
Imports Pooling2DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling2DConfig
Imports Pooling3DConfig = org.nd4j.linalg.api.ops.impl.layers.convolution.config.Pooling3DConfig
Imports Nd4jBackend = org.nd4j.linalg.factory.Nd4jBackend

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

Namespace org.nd4j.autodiff.samediff

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Tag(TagNames.JAVA_ONLY) public class ConvConfigTests extends org.nd4j.linalg.BaseNd4jTestWithBackends
	Public Class ConvConfigTests
		Inherits BaseNd4jTestWithBackends

		Public Overrides Function ordering() As Char
			Return "c"c
		End Function

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDeConv2D(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDeConv2D(ByVal backend As Nd4jBackend)
			DeConv2DConfig.builder().kH(2).kW(4).build()

			Try
				DeConv2DConfig.builder().kW(4).kH(0).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Kernel height"))
			End Try

			Try
				DeConv2DConfig.builder().kH(4).kW(0).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Kernel width"))
			End Try

			Try
				DeConv2DConfig.builder().kH(4).kW(3).sH(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Stride height"))
			End Try

			Try
				DeConv2DConfig.builder().kH(4).kW(3).sW(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Stride width"))
			End Try

			Try
				DeConv2DConfig.builder().kH(4).kW(3).pH(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Padding height"))
			End Try

			Try
				DeConv2DConfig.builder().kH(4).kW(3).pW(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Padding width"))
			End Try

			Try
				DeConv2DConfig.builder().kH(4).kW(3).dH(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Dilation height"))
			End Try

			Try
				DeConv2DConfig.builder().kH(4).kW(3).dW(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Dilation width"))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConv2D(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConv2D(ByVal backend As Nd4jBackend)
			Conv2DConfig.builder().kH(2).kW(4).build()

			Try
				Conv2DConfig.builder().kW(4).kH(0).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Kernel height"))
			End Try

			Try
				Conv2DConfig.builder().kH(4).kW(0).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Kernel width"))
			End Try

			Try
				Conv2DConfig.builder().kH(4).kW(3).sH(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Stride height"))
			End Try

			Try
				Conv2DConfig.builder().kH(4).kW(3).sW(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Stride width"))
			End Try

			Try
				Conv2DConfig.builder().kH(4).kW(3).pH(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Padding height"))
			End Try

			Try
				Conv2DConfig.builder().kH(4).kW(3).pW(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Padding width"))
			End Try

			Try
				Conv2DConfig.builder().kH(4).kW(3).dH(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Dilation height"))
			End Try

			Try
				Conv2DConfig.builder().kH(4).kW(3).dW(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Dilation width"))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPooling2D(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPooling2D(ByVal backend As Nd4jBackend)
			Pooling2DConfig.builder().kH(2).kW(4).build()

			Try
				Pooling2DConfig.builder().kW(4).kH(0).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Kernel height"))
			End Try

			Try
				Pooling2DConfig.builder().kH(4).kW(0).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Kernel width"))
			End Try

			Try
				Pooling2DConfig.builder().kH(4).kW(3).sH(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Stride height"))
			End Try

			Try
				Pooling2DConfig.builder().kH(4).kW(3).sW(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Stride width"))
			End Try

			Try
				Pooling2DConfig.builder().kH(4).kW(3).pH(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Padding height"))
			End Try

			Try
				Pooling2DConfig.builder().kH(4).kW(3).pW(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Padding width"))
			End Try

			Try
				Pooling2DConfig.builder().kH(4).kW(3).dH(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Dilation height"))
			End Try

			Try
				Pooling2DConfig.builder().kH(4).kW(3).dW(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Dilation width"))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testDeConv3D(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testDeConv3D(ByVal backend As Nd4jBackend)
			DeConv3DConfig.builder().kH(2).kW(4).kD(3).build()

			Try
				DeConv3DConfig.builder().kW(4).kD(3).kH(0).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Kernel height"))
			End Try

			Try
				DeConv3DConfig.builder().kH(4).kD(3).kW(0).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Kernel width"))
			End Try

			Try
				DeConv3DConfig.builder().kH(4).kW(3).kD(0).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Kernel depth"))
			End Try

			Try
				DeConv3DConfig.builder().kH(4).kW(3).kD(3).sH(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Stride height"))
			End Try

			Try
				DeConv3DConfig.builder().kH(4).kW(3).kD(3).sW(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Stride width"))
			End Try

			Try
				DeConv3DConfig.builder().kH(4).kW(3).kD(3).sD(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Stride depth"))
			End Try

			Try
				DeConv3DConfig.builder().kH(4).kW(3).kD(3).pH(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Padding height"))
			End Try

			Try
				DeConv3DConfig.builder().kH(4).kW(3).kD(3).pW(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Padding width"))
			End Try

			Try
				DeConv3DConfig.builder().kH(4).kW(3).kD(3).pD(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Padding depth"))
			End Try

			Try
				DeConv3DConfig.builder().kH(4).kW(3).kD(3).dH(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Dilation height"))
			End Try

			Try
				DeConv3DConfig.builder().kH(4).kW(3).kD(3).dW(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Dilation width"))
			End Try

			Try
				DeConv3DConfig.builder().kH(4).kW(3).kD(3).dD(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Dilation depth"))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConv3D(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testConv3D(ByVal backend As Nd4jBackend)
			Conv3DConfig.builder().kH(2).kW(4).kD(3).build()

			Try
				Conv3DConfig.builder().kW(4).kD(3).kH(0).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Kernel height"))
			End Try

			Try
				Conv3DConfig.builder().kH(4).kD(3).kW(0).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Kernel width"))
			End Try

			Try
				Conv3DConfig.builder().kH(4).kW(3).kD(0).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Kernel depth"))
			End Try

			Try
				Conv3DConfig.builder().kH(4).kW(3).kD(3).sH(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Stride height"))
			End Try

			Try
				Conv3DConfig.builder().kH(4).kW(3).kD(3).sW(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Stride width"))
			End Try

			Try
				Conv3DConfig.builder().kH(4).kW(3).kD(3).sD(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Stride depth"))
			End Try

			Try
				Conv3DConfig.builder().kH(4).kW(3).kD(3).pH(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Padding height"))
			End Try

			Try
				Conv3DConfig.builder().kH(4).kW(3).kD(3).pW(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Padding width"))
			End Try

			Try
				Conv3DConfig.builder().kH(4).kW(3).kD(3).pD(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Padding depth"))
			End Try

			Try
				Conv3DConfig.builder().kH(4).kW(3).kD(3).dH(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Dilation height"))
			End Try

			Try
				Conv3DConfig.builder().kH(4).kW(3).kD(3).dW(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Dilation width"))
			End Try

			Try
				Conv3DConfig.builder().kH(4).kW(3).kD(3).dD(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Dilation depth"))
			End Try
		End Sub



'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testPooling3D(org.nd4j.linalg.factory.Nd4jBackend backend)
		Public Overridable Sub testPooling3D(ByVal backend As Nd4jBackend)
			Pooling3DConfig.builder().kH(2).kW(4).kD(3).build()

			Try
				Pooling3DConfig.builder().kW(4).kD(3).kH(0).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Kernel height"))
			End Try

			Try
				Pooling3DConfig.builder().kH(4).kD(3).kW(0).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Kernel width"))
			End Try

			Try
				Pooling3DConfig.builder().kH(4).kW(3).kD(0).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Kernel depth"))
			End Try

			Try
				Pooling3DConfig.builder().kH(4).kW(3).kD(3).sH(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Stride height"))
			End Try

			Try
				Pooling3DConfig.builder().kH(4).kW(3).kD(3).sW(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Stride width"))
			End Try

			Try
				Pooling3DConfig.builder().kH(4).kW(3).kD(3).sD(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Stride depth"))
			End Try

			Try
				Pooling3DConfig.builder().kH(4).kW(3).kD(3).pH(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Padding height"))
			End Try

			Try
				Pooling3DConfig.builder().kH(4).kW(3).kD(3).pW(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Padding width"))
			End Try

			Try
				Pooling3DConfig.builder().kH(4).kW(3).kD(3).pD(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Padding depth"))
			End Try

			Try
				Pooling3DConfig.builder().kH(4).kW(3).kD(3).dH(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Dilation height"))
			End Try

			Try
				Pooling3DConfig.builder().kH(4).kW(3).kD(3).dW(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Dilation width"))
			End Try

			Try
				Pooling3DConfig.builder().kH(4).kW(3).kD(3).dD(-2).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Dilation depth"))
			End Try
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @ParameterizedTest @MethodSource("org.nd4j.linalg.BaseNd4jTestWithBackends#configs") public void testConv1D()
		Public Overridable Sub testConv1D()
			Conv1DConfig.builder().k(2).paddingMode(PaddingMode.SAME).build()

			Try
				Conv1DConfig.builder().k(0).paddingMode(PaddingMode.SAME).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Kernel"))
			End Try

			Try
				Conv1DConfig.builder().k(4).s(-2).paddingMode(PaddingMode.SAME).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Stride"))
			End Try

			Try
				Conv1DConfig.builder().k(3).p(-2).paddingMode(PaddingMode.SAME).build()
				fail()
			Catch e As System.ArgumentException
				assertTrue(e.Message.contains("Padding"))
			End Try
		End Sub
	End Class

End Namespace