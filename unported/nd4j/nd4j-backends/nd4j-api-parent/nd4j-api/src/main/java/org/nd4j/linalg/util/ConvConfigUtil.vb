Imports AccessLevel = lombok.AccessLevel
Imports NoArgsConstructor = lombok.NoArgsConstructor
Imports Preconditions = org.nd4j.common.base.Preconditions

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

Namespace org.nd4j.linalg.util

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @NoArgsConstructor(access = AccessLevel.@PRIVATE) public class ConvConfigUtil
	Public Class ConvConfigUtil

		''' <summary>
		''' Validate a 2D convolution's Kernel, Stride, Padding, and Dilation
		''' </summary>
		Public Shared Sub validate2D(ByVal kH As Long, ByVal kW As Long, ByVal sH As Long, ByVal sW As Long, ByVal pH As Long, ByVal pW As Long, ByVal dH As Long, ByVal dW As Long)
			Preconditions.checkArgument(kH <> 0, "Kernel height can not be 0")
			Preconditions.checkArgument(kW <> 0, "Kernel width can not be 0")

			Preconditions.checkArgument(sH > 0, "Stride height can not be negative or 0, got: %s", sH)
			Preconditions.checkArgument(sW > 0, "Stride width can not be negative or 0, got: %s", sW)

			Preconditions.checkArgument(pH >= 0, "Padding height can not be negative, got: %s", pH)
			Preconditions.checkArgument(pW >= 0, "Padding width can not be negative, got: %s", pW)

			Preconditions.checkArgument(dH > 0, "Dilation height can not be negative or 0, got: %s", dH)
			Preconditions.checkArgument(dW > 0, "Dilation width can not be negative or 0, got: %s", dW)
		End Sub

		''' <summary>
		''' Validate a 3D convolution's Kernel, Stride, Padding, and Dilation
		''' </summary>
		Public Shared Sub validate3D(ByVal kH As Long, ByVal kW As Long, ByVal kD As Long, ByVal sH As Long, ByVal sW As Long, ByVal sD As Long, ByVal pH As Long, ByVal pW As Long, ByVal pD As Long, ByVal dH As Long, ByVal dW As Long, ByVal dD As Long)
			Preconditions.checkArgument(kH <> 0, "Kernel height can not be 0")
			Preconditions.checkArgument(kW <> 0, "Kernel width can not be 0")
			Preconditions.checkArgument(kD <> 0, "Kernel depth can not be 0")

			Preconditions.checkArgument(sH > 0, "Stride height can not be negative or 0, got: %s", sH)
			Preconditions.checkArgument(sW > 0, "Stride width can not be negative or 0, got: %s", sW)
			Preconditions.checkArgument(sD > 0, "Stride depth can not be negative or 0, got: %s", sD)

			Preconditions.checkArgument(pH >= 0, "Padding height can not be negative, got: %s", pH)
			Preconditions.checkArgument(pW >= 0, "Padding width can not be negative, got: %s", pW)
			Preconditions.checkArgument(pD >= 0, "Padding depth can not be negative, got: %s", pD)

			Preconditions.checkArgument(dH > 0, "Dilation height can not be negative or 0, got: %s", dH)
			Preconditions.checkArgument(dW > 0, "Dilation width can not be negative or 0, got: %s", dW)
			Preconditions.checkArgument(dD > 0, "Dilation depth can not be negative or 0, got: %s", dD)
		End Sub

		''' <summary>
		''' Validate a 3D convolution's Output Padding
		''' </summary>
		Public Shared Sub validateExtra3D(ByVal aH As Long, ByVal aW As Long, ByVal aD As Long)
			Preconditions.checkArgument(aH >= 0, "Output padding height can not be negative, got: %s", aH)
			Preconditions.checkArgument(aW >= 0, "Output padding width can not be negative, got: %s", aW)
			Preconditions.checkArgument(aD >= 0, "Output padding depth can not be negative, got: %s", aD)
		End Sub

		''' <summary>
		''' Validate a 1D convolution's Kernel, Stride, and Padding
		''' </summary>
		Public Shared Sub validate1D(ByVal k As Long, ByVal s As Long, ByVal p As Long, ByVal d As Long)
			Preconditions.checkArgument(k <> 0, "Kernel can not be 0")

			Preconditions.checkArgument(s > 0, "Stride can not be negative or 0, got: %s", s)

			Preconditions.checkArgument(d > 0, "Dilation can not be negative or 0, got: %s", s)

			Preconditions.checkArgument(p >= 0, "Padding can not be negative, got: %s", p)
		End Sub

		''' <summary>
		''' Validate a LocalResponseNormalizationConfig
		''' </summary>
		Public Shared Sub validateLRN(ByVal alpha As Double, ByVal beta As Double, ByVal bias As Double, ByVal depth As Integer)
			Preconditions.checkArgument(depth > 0, "Depth can not be 0 or negative, got: %s", depth)
		End Sub
	End Class

End Namespace