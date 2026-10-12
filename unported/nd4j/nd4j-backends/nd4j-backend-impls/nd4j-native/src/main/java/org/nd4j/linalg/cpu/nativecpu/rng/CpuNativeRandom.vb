Imports PointerPointer = org.bytedeco.javacpp.PointerPointer
Imports NativeOps = org.nd4j.nativeblas.NativeOps
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports OpaqueRandomGenerator = org.nd4j.nativeblas.OpaqueRandomGenerator
Imports NativeRandom = org.nd4j.rng.NativeRandom

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

Namespace org.nd4j.linalg.cpu.nativecpu.rng

	Public Class CpuNativeRandom
		Inherits NativeRandom

		Private Shadows nativeOps As NativeOps

		Public Sub New()
			MyBase.New()
		End Sub

		Public Sub New(ByVal seed As Long)
			MyBase.New(seed)
		End Sub

		Public Sub New(ByVal seed As Long, ByVal numberOfElements As Long)
			MyBase.New(seed, numberOfElements)
		End Sub

		Public Overrides Sub init()
			nativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()
			statePointer_Conflict = nativeOps.createRandomGenerator(Me.seed_Conflict, Me.seed_Conflict Xor &HdeadbeefL)
		End Sub

		Public Overrides Sub close()
			nativeOps.deleteRandomGenerator(DirectCast(statePointer_Conflict, OpaqueRandomGenerator))
		End Sub

		Public Overrides ReadOnly Property ExtraPointers As PointerPointer
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides Property Seed As Long
			Set(ByVal seed As Long)
				Me.seed_Conflict = seed
				Me.currentPosition.set(0)
				nativeOps.setRandomGeneratorStates(DirectCast(statePointer_Conflict, OpaqueRandomGenerator), seed, seed Xor &HdeadbeefL)
			End Set
			Get
				Return seed_Conflict
			End Get
		End Property


		Public Overrides Function nextInt() As Integer
			Return nativeOps.getRandomGeneratorRelativeInt(DirectCast(statePointer_Conflict, OpaqueRandomGenerator), currentPosition.getAndIncrement())
		End Function

		Public Overrides Function nextFloat() As Single
			Return nativeOps.getRandomGeneratorRelativeFloat(DirectCast(statePointer_Conflict, OpaqueRandomGenerator), currentPosition.getAndIncrement())
		End Function

		Public Overrides Function nextDouble() As Double
			Return nativeOps.getRandomGeneratorRelativeDouble(DirectCast(statePointer_Conflict, OpaqueRandomGenerator), currentPosition.getAndIncrement())
		End Function

		Public Overrides Function nextLong() As Long
			Return nativeOps.getRandomGeneratorRelativeLong(DirectCast(statePointer_Conflict, OpaqueRandomGenerator), currentPosition.getAndIncrement())
		End Function

		Public Overrides Function rootState() As Long
			Return nativeOps.getRandomGeneratorRootState(DirectCast(statePointer_Conflict, OpaqueRandomGenerator))
		End Function

		Public Overrides Function nodeState() As Long
			Return nativeOps.getRandomGeneratorNodeState(DirectCast(statePointer_Conflict, OpaqueRandomGenerator))
		End Function

		Public Overrides Sub setStates(ByVal rootState As Long, ByVal nodeState As Long)
			nativeOps.setRandomGeneratorStates(DirectCast(statePointer_Conflict, OpaqueRandomGenerator), rootState, nodeState)
		End Sub
	End Class

End Namespace