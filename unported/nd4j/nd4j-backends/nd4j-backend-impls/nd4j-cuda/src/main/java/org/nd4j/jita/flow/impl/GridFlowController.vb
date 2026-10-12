Imports AllocationPoint = org.nd4j.jita.allocator.impl.AllocationPoint
Imports GridExecutioner = org.nd4j.linalg.api.ops.executioner.GridExecutioner
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.nd4j.jita.flow.impl

	''' 
	''' <summary>
	''' FlowController implementation suitable for CudaGridExecutioner
	''' 
	''' Main difference here, is delayed execution support and forced execution trigger in special cases
	''' 
	''' @author raver119@gmail.com
	''' </summary>
	Public Class GridFlowController
		Inherits SynchronousFlowController

		Private Shared logger As Logger = LoggerFactory.getLogger(GetType(GridFlowController))

		''' <summary>
		''' This method makes sure HOST memory contains latest data from GPU
		''' 
		''' Additionally, this method checks, that there's no ops pending execution for this array
		''' </summary>
		''' <param name="point"> </param>
		Public Overrides Sub synchronizeToHost(ByVal point As AllocationPoint)
			If Not point.isConstant() AndAlso point.Enqueued Then
				waitTillFinished(point)
			End If

			MyBase.synchronizeToHost(point)
		End Sub

		''' 
		''' <summary>
		''' Additionally, this method checks, that there's no ops pending execution for this array </summary>
		''' <param name="point"> </param>
		Public Overrides Sub waitTillFinished(ByVal point As AllocationPoint)
			If Not point.isConstant() AndAlso point.Enqueued Then
				Nd4j.Executioner.commit()
			End If

			MyBase.waitTillFinished(point)
		End Sub

		''' 
		''' <summary>
		''' Additionally, this method checks, that there's no ops pending execution for this array
		''' </summary>
		''' <param name="point"> </param>
		Public Overrides Sub waitTillReleased(ByVal point As AllocationPoint)
			''' <summary>
			''' We don't really need special hook here, because if op is enqueued - it's still holding all arrays
			''' </summary>

			MyBase.waitTillReleased(point)
		End Sub
	End Class

End Namespace