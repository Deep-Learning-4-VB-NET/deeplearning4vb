Imports System.Threading
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext

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

Namespace org.nd4j.jita.allocator.garbage


	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
	Public Class GarbageResourceReference
		Inherits WeakReference(Of Thread)

'JAVA TO VB CONVERTER NOTE: The field context was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly context_Conflict As CudaContext
'JAVA TO VB CONVERTER NOTE: The field threadId was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly threadId_Conflict As Long
'JAVA TO VB CONVERTER NOTE: The field deviceId was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly deviceId_Conflict As Integer

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to the Java 'super' constraint:
'ORIGINAL LINE: public GarbageResourceReference(Thread referent, java.lang.ref.ReferenceQueue<? super Thread> q, org.nd4j.linalg.jcublas.context.CudaContext context, int deviceId)
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
		Public Sub New(ByVal referent As Thread, ByVal q As ReferenceQueue(Of T1), ByVal context As CudaContext, ByVal deviceId As Integer)
			MyBase.New(referent, q)
			Me.context_Conflict = context
			Me.threadId_Conflict = referent.getId()
			Me.deviceId_Conflict = deviceId
		End Sub

		Public Overridable ReadOnly Property Context As CudaContext
			Get
				Return context_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property ThreadId As Long
			Get
				Return threadId_Conflict
			End Get
		End Property

		Public Overridable ReadOnly Property DeviceId As Integer
			Get
				Return deviceId_Conflict
			End Get
		End Property
	End Class

End Namespace