Imports System
Imports System.Collections.Generic
Imports ArrayHolder = org.nd4j.autodiff.samediff.ArrayHolder
Imports SDVariable = org.nd4j.autodiff.samediff.SDVariable
Imports SameDiff = org.nd4j.autodiff.samediff.SameDiff
Imports SameDiffOp = org.nd4j.autodiff.samediff.internal.SameDiffOp
Imports OptimizationHelper = org.nd4j.autodiff.samediff.optimize.OptimizationHelper
Imports Optimizer = org.nd4j.autodiff.samediff.optimize.Optimizer
Imports Conv2D = org.nd4j.linalg.api.ops.impl.layers.convolution.Conv2D
Imports Nd4j = org.nd4j.linalg.factory.Nd4j

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

Namespace org.nd4j.autodiff.samediff.optimize.optimizations


	Public Class CuDNNFunctionOptimizations
		Inherits BaseOptimizerSet

		Protected Friend Shared ReadOnly isCudaBackend As Boolean

		Shared Sub New()
			Dim backend As String = Nd4j.Executioner.EnvironmentInformation.getProperty("backend")
			isCudaBackend = "CUDA".Equals(backend, StringComparison.OrdinalIgnoreCase)
		End Sub

		''' <summary>
		''' https://docs.nvidia.com/deeplearning/sdk/dl-performance-guide/index.html#tensor-layout
		''' For tensor cores: we want NHWC layout:
		''' Section 7.3.1
		''' "Layout choice has an effect on performance, as convolutions implemented for Tensor Cores require NHWC layout and are fastest when input tensors are laid out in NHWC."
		''' "To maximize performance, we recommend using NHWC tensor layout."
		''' 
		''' As for weights format: cuDNN docs are vague - but TF uses NCHW+OIHW or NHWC+OHWI
		''' </summary>
		Public Class CudnnConv2dNCHWtoNHWCConversion
			Implements Optimizer

			Public Overridable Function checkAndApply(ByVal sd As SameDiff, ByVal helper As OptimizationHelper, ByVal op As SameDiffOp, ByVal constantArrays As ArrayHolder, ByVal variablesArrays As ArrayHolder) As Boolean
				If Not (TypeOf op.Op Is Conv2D) Then
					Return False
				End If

				Dim c2d As Conv2D = DirectCast(op.Op, Conv2D)
				Dim weightsCorrect As Boolean = False
				Dim activationsCorrect As Boolean = c2d.getConfig().isNHWC()

				If activationsCorrect AndAlso weightsCorrect Then
					Return False 'Nothing to do here
				End If

				'Now, we need to do 2 things
				'(a) replace NCHW to NHWC for input
				'(b) set weight format to OHWI (OYXI)

				Dim inputs As IList(Of String) = op.getInputsToOp()
				Dim wArgName As String = inputs(1)

				'Step 1 - replace activations
				If Not activationsCorrect Then
					Dim inArgName As String = inputs(0)
					Dim [in] As SDVariable = sd.getVariable(inArgName)
					'Replace [in -> Conv2d(NCHW) -> out] with [in -> permute -> Conv2d(NHWC) -> permute -> out]
					Dim newName As String = [in].name() & "_cudnn_nchw_to_nhwc"
					OptimizationUtils.replaceOpInputsWith(sd, [in].name(), newName)
					Dim nhwc As SDVariable = [in].permute(0, 2, 3, 1).rename(newName) 'NCHW to NHWC

					Dim outNhwc As SDVariable = sd.getVariable(op.getOutputsOfOp()(0))
					Dim newName2 As String = outNhwc.name() & "_cudnn_nhwc_to_nchw"
					Dim outNchw As SDVariable = outNhwc.permute(0, 3, 1, 2).rename(newName2) 'NHWC to NCHW

					OptimizationUtils.replaceOpInputsWith(sd, outNhwc.name(), outNchw.name())

					c2d.getConfig().isNHWC(True)
				End If

				'Step 2 - replace YXIO weights (default) with OYXI weights
				'We'll just add a permute here, and let other optimizer steps fix the (variable -> permute -> op ==> permutedVariable -> op) part
				If Not weightsCorrect Then
					Dim w As SDVariable = sd.getVariable(wArgName)
					Dim newWname As String = w.name() & "_cudnn_yxio_to_oyxi"
					OptimizationUtils.replaceOpInputsWith(sd, w.name(), newWname)
					Dim wPermuted As SDVariable = w.permute(3, 0, 1, 2).rename(newWname)


					'TODO once config supports weight layout, set it here
				End If


				Return True
			End Function
		End Class

	'    
	'    TODO: Also do pooling2d, batchnorm, etc
	'     

	End Class

End Namespace