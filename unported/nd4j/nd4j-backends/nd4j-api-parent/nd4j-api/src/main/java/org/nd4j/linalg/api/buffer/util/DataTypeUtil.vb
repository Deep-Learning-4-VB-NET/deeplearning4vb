Imports System
Imports Nd4jContext = org.nd4j.context.Nd4jContext
Imports DataType = org.nd4j.linalg.api.buffer.DataType

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


	Public Class DataTypeUtil

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: private volatile transient static org.nd4j.linalg.api.buffer.DataType dtype;
		<NonSerialized>
		Private Shared dtype As DataType
		Private Shared ReadOnly lock As ReadWriteLock = New ReentrantReadWriteLock()


		''' <summary>
		''' Returns the length for the given data opType </summary>
		''' <param name="type">
		''' @return </param>
		Public Shared Function lengthForDtype(ByVal type As DataType) As Integer
			Select Case type.innerEnumValue
				Case DataType.InnerEnum.DOUBLE
					Return 8
				Case DataType.InnerEnum.FLOAT
					Return 4
				Case DataType.InnerEnum.INT
					Return 4
				Case DataType.InnerEnum.HALF
					Return 2
				Case DataType.InnerEnum.LONG
					Return 8
				Case Else
					Throw New System.ArgumentException("Illegal opType for length")

			End Select


		End Function

		''' <summary>
		''' Get the allocation mode from the context
		''' @return
		''' </summary>
		Public Shared Function getDtypeFromContext(ByVal dType As String) As DataType
			Select Case dType
				Case "double"
					Return DataType.DOUBLE
				Case "float"
					Return DataType.FLOAT
				Case "int"
					Return DataType.INT
				Case "half"
					Return DataType.HALF
				Case Else
					Return DataType.FLOAT
			End Select
		End Function

		''' <summary>
		''' Gets the name of the alocation mode </summary>
		''' <param name="allocationMode">
		''' @return </param>
		Public Shared Function getDTypeForName(ByVal allocationMode As DataType) As String
			Select Case allocationMode.innerEnumValue
				Case DataType.InnerEnum.DOUBLE
					Return "double"
				Case DataType.InnerEnum.FLOAT
					Return "float"
				Case DataType.InnerEnum.INT
					Return "int"
				Case DataType.InnerEnum.HALF
					Return "half"
				Case Else
					Return "float"
			End Select
		End Function

		''' <summary>
		''' get the allocation mode from the context
		''' @return
		''' </summary>
		Public Shared ReadOnly Property DtypeFromContext As DataType
			Get
				Try
					lock.readLock().lock()
    
					If dtype = Nothing Then
						lock.readLock().unlock()
						lock.writeLock().lock()
    
						If dtype = Nothing Then
							dtype = getDtypeFromContext(Nd4jContext.Instance.Conf.getProperty("dtype"))
						End If
    
						lock.writeLock().unlock()
						lock.readLock().lock()
					End If
    
					Return dtype
				Finally
					lock.readLock().unlock()
				End Try
			End Get
		End Property

		''' <summary>
		''' Set the allocation mode for the nd4j context
		''' The value must be one of: heap, java cpp, or direct
		''' or an @link{IllegalArgumentException} is thrown </summary>
		''' <param name="allocationModeForContext"> </param>
		Public Shared WriteOnly Property DTypeForContext As DataType
			Set(ByVal allocationModeForContext As DataType)
				Try
					lock.writeLock().lock()
    
					dtype = allocationModeForContext
    
					setDTypeForContext(getDTypeForName(allocationModeForContext))
				Finally
					lock.writeLock().unlock()
				End Try
			End Set
		End Property

		''' <summary>
		''' Set the allocation mode for the nd4j context
		''' The value must be one of: heap, java cpp, or direct
		''' or an @link{IllegalArgumentException} is thrown </summary>
		''' <param name="allocationModeForContext"> </param>
		Public Shared WriteOnly Property DTypeForContext As String
			Set(ByVal allocationModeForContext As String)
				If Not allocationModeForContext.Equals("double") AndAlso Not allocationModeForContext.Equals("float") AndAlso Not allocationModeForContext.Equals("int") AndAlso Not allocationModeForContext.Equals("half") Then
					Throw New System.ArgumentException("Allocation mode must be one of: double,float, or int")
				End If
				Nd4jContext.Instance.Conf.put("dtype", allocationModeForContext)
			End Set
		End Property


	End Class

End Namespace