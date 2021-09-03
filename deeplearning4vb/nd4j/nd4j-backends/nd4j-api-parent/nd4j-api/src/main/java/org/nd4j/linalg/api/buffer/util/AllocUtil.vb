Imports Nd4jContext = org.nd4j.context.Nd4jContext
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer

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

Namespace org.nd4j.linalg.api.buffer.util

	Public Class AllocUtil


		''' <summary>
		''' Get the allocation mode from the context
		''' @return
		''' </summary>
		Public Shared Function getAllocationModeFromContext(ByVal allocMode As String) As DataBuffer.AllocationMode
			Select Case allocMode
				Case "heap"
					Return DataBuffer.AllocationMode.HEAP
				Case "javacpp"
					Return DataBuffer.AllocationMode.JAVACPP
				Case "direct"
					Return DataBuffer.AllocationMode.DIRECT
				Case Else
					Return DataBuffer.AllocationMode.JAVACPP
			End Select
		End Function

		''' <summary>
		''' Gets the name of the alocation mode </summary>
		''' <param name="allocationMode">
		''' @return </param>
		Public Shared Function getAllocModeName(ByVal allocationMode As DataBuffer.AllocationMode) As String
			Select Case allocationMode
				Case DataBuffer.AllocationMode.HEAP
					Return "heap"
				Case DataBuffer.AllocationMode.JAVACPP
					Return "javacpp"
				Case DataBuffer.AllocationMode.DIRECT
					Return "direct"
				Case Else
					Return "javacpp"
			End Select
		End Function

		''' <summary>
		''' get the allocation mode from the context
		''' @return
		''' </summary>
		Public Shared ReadOnly Property AllocationModeFromContext As DataBuffer.AllocationMode
			Get
				Return DataBuffer.AllocationMode.MIXED_DATA_TYPES 'getAllocationModeFromContext(Nd4jContext.getInstance().getConf().getProperty("alloc"));
			End Get
		End Property

		''' <summary>
		''' Set the allocation mode for the nd4j context
		''' The value must be one of: heap, java cpp, or direct
		''' or an @link{IllegalArgumentException} is thrown </summary>
		''' <param name="allocationModeForContext"> </param>
		Public Shared WriteOnly Property AllocationModeForContext As DataBuffer.AllocationMode
			Set(ByVal allocationModeForContext As DataBuffer.AllocationMode)
				setAllocationModeForContext(getAllocModeName(allocationModeForContext))
			End Set
		End Property

		''' <summary>
		''' Set the allocation mode for the nd4j context
		''' The value must be one of: heap, java cpp, or direct
		''' or an @link{IllegalArgumentException} is thrown </summary>
		''' <param name="allocationModeForContext"> </param>
		Public Shared WriteOnly Property AllocationModeForContext As String
			Set(ByVal allocationModeForContext As String)
				If Not allocationModeForContext.Equals("heap") AndAlso Not allocationModeForContext.Equals("javacpp") AndAlso Not allocationModeForContext.Equals("direct") Then
					Throw New System.ArgumentException("Allocation mode must be one of: heap,javacpp, or direct")
				End If
				Nd4jContext.Instance.Conf.put("alloc", allocationModeForContext)
			End Set
		End Property

	End Class

End Namespace