Imports AllocationPoint = org.nd4j.jita.allocator.impl.AllocationPoint
Imports BaseDataBuffer = org.nd4j.linalg.api.buffer.BaseDataBuffer

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
	Public Class GarbageBufferReference
		Inherits WeakReference(Of BaseDataBuffer)

'JAVA TO VB CONVERTER NOTE: The field point was renamed since Visual Basic does not allow fields to have the same name as other class members:
		Private ReadOnly point_Conflict As AllocationPoint

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to the Java 'super' constraint:
'ORIGINAL LINE: public GarbageBufferReference(org.nd4j.linalg.api.buffer.BaseDataBuffer referent, java.lang.ref.ReferenceQueue<? super org.nd4j.linalg.api.buffer.BaseDataBuffer> q, org.nd4j.jita.allocator.impl.AllocationPoint point)
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
		Public Sub New(ByVal referent As BaseDataBuffer, ByVal q As ReferenceQueue(Of T1), ByVal point As AllocationPoint)
			MyBase.New(referent, q)
			Me.point_Conflict = point
		End Sub

		Public Overridable ReadOnly Property Point As AllocationPoint
			Get
				Return point_Conflict
			End Get
		End Property
	End Class

End Namespace