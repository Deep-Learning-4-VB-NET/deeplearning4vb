Imports Data = lombok.Data
Imports Deallocatable = org.nd4j.linalg.api.memory.Deallocatable
Imports Deallocator = org.nd4j.linalg.api.memory.Deallocator

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

Namespace org.nd4j.linalg.api.memory.deallocation


'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Data public class DeallocatableReference extends java.lang.ref.WeakReference<org.nd4j.linalg.api.memory.Deallocatable>
	Public Class DeallocatableReference
		Inherits WeakReference(Of Deallocatable)

		Private id As String
		Private deallocator As Deallocator

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to the Java 'super' constraint:
'ORIGINAL LINE: public DeallocatableReference(org.nd4j.linalg.api.memory.Deallocatable referent, java.lang.ref.ReferenceQueue<? super org.nd4j.linalg.api.memory.Deallocatable> q)
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
		Public Sub New(ByVal referent As Deallocatable, ByVal q As ReferenceQueue(Of T1))
			MyBase.New(referent, q)

			Me.id = referent.UniqueId
			Me.deallocator = referent.deallocator()
		End Sub
	End Class

End Namespace