Imports Getter = lombok.Getter
Imports Pointer = org.bytedeco.javacpp.Pointer

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

Namespace org.nd4j.rng.deallocator


	Public Class GarbageStateReference
		Inherits WeakReference(Of NativePack)

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter private org.bytedeco.javacpp.Pointer statePointer;
		Private statePointer As Pointer

'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to the Java 'super' constraint:
'ORIGINAL LINE: public GarbageStateReference(NativePack referent, java.lang.ref.ReferenceQueue<? super NativePack> queue)
'JAVA TO VB CONVERTER TODO TASK: Wildcard generics in constructor parameters are not converted. Move the generic type parameter and constraint to the class header:
		Public Sub New(ByVal referent As NativePack, ByVal queue As ReferenceQueue(Of T1))
			MyBase.New(referent, queue)
			Me.statePointer = referent.getStatePointer()
			If Me.statePointer Is Nothing Then
				Throw New System.InvalidOperationException("statePointer shouldn't be NULL")
			End If
		End Sub
	End Class

End Namespace