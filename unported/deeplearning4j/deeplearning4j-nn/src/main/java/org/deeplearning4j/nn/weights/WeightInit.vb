Imports System.Collections.Generic
Imports Distribution = org.deeplearning4j.nn.conf.distribution.Distribution

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

Namespace org.deeplearning4j.nn.weights

	Public NotInheritable Class WeightInit
		Public Shared ReadOnly DISTRIBUTION As New WeightInit("DISTRIBUTION", InnerEnum.DISTRIBUTION)
		Public Shared ReadOnly ZERO As New WeightInit("ZERO", InnerEnum.ZERO)
		Public Shared ReadOnly ONES As New WeightInit("ONES", InnerEnum.ONES)
		Public Shared ReadOnly SIGMOID_UNIFORM As New WeightInit("SIGMOID_UNIFORM", InnerEnum.SIGMOID_UNIFORM)
		Public Shared ReadOnly NORMAL As New WeightInit("NORMAL", InnerEnum.NORMAL)
		Public Shared ReadOnly LECUN_NORMAL As New WeightInit("LECUN_NORMAL", InnerEnum.LECUN_NORMAL)
		Public Shared ReadOnly UNIFORM As New WeightInit("UNIFORM", InnerEnum.UNIFORM)
		Public Shared ReadOnly XAVIER As New WeightInit("XAVIER", InnerEnum.XAVIER)
		Public Shared ReadOnly XAVIER_UNIFORM As New WeightInit("XAVIER_UNIFORM", InnerEnum.XAVIER_UNIFORM)
		Public Shared ReadOnly XAVIER_FAN_IN As New WeightInit("XAVIER_FAN_IN", InnerEnum.XAVIER_FAN_IN)
		Public Shared ReadOnly XAVIER_LEGACY As New WeightInit("XAVIER_LEGACY", InnerEnum.XAVIER_LEGACY)
		Public Shared ReadOnly RELU As New WeightInit("RELU", InnerEnum.RELU)
		Public Shared ReadOnly RELU_UNIFORM As New WeightInit("RELU_UNIFORM", InnerEnum.RELU_UNIFORM)
		Public Shared ReadOnly IDENTITY As New WeightInit("IDENTITY", InnerEnum.IDENTITY)
		Public Shared ReadOnly LECUN_UNIFORM As New WeightInit("LECUN_UNIFORM", InnerEnum.LECUN_UNIFORM)
		Public Shared ReadOnly VAR_SCALING_NORMAL_FAN_IN As New WeightInit("VAR_SCALING_NORMAL_FAN_IN", InnerEnum.VAR_SCALING_NORMAL_FAN_IN)
		Public Shared ReadOnly VAR_SCALING_NORMAL_FAN_OUT As New WeightInit("VAR_SCALING_NORMAL_FAN_OUT", InnerEnum.VAR_SCALING_NORMAL_FAN_OUT)
		Public Shared ReadOnly VAR_SCALING_NORMAL_FAN_AVG As New WeightInit("VAR_SCALING_NORMAL_FAN_AVG", InnerEnum.VAR_SCALING_NORMAL_FAN_AVG)
		Public Shared ReadOnly VAR_SCALING_UNIFORM_FAN_IN As New WeightInit("VAR_SCALING_UNIFORM_FAN_IN", InnerEnum.VAR_SCALING_UNIFORM_FAN_IN)
		Public Shared ReadOnly VAR_SCALING_UNIFORM_FAN_OUT As New WeightInit("VAR_SCALING_UNIFORM_FAN_OUT", InnerEnum.VAR_SCALING_UNIFORM_FAN_OUT)
		Public Shared ReadOnly VAR_SCALING_UNIFORM_FAN_AVG As New WeightInit("VAR_SCALING_UNIFORM_FAN_AVG", InnerEnum.VAR_SCALING_UNIFORM_FAN_AVG)

		Private Shared ReadOnly valueList As New List(Of WeightInit)()

		Shared Sub New()
			valueList.Add(DISTRIBUTION)
			valueList.Add(ZERO)
			valueList.Add(ONES)
			valueList.Add(SIGMOID_UNIFORM)
			valueList.Add(NORMAL)
			valueList.Add(LECUN_NORMAL)
			valueList.Add(UNIFORM)
			valueList.Add(XAVIER)
			valueList.Add(XAVIER_UNIFORM)
			valueList.Add(XAVIER_FAN_IN)
			valueList.Add(XAVIER_LEGACY)
			valueList.Add(RELU)
			valueList.Add(RELU_UNIFORM)
			valueList.Add(IDENTITY)
			valueList.Add(LECUN_UNIFORM)
			valueList.Add(VAR_SCALING_NORMAL_FAN_IN)
			valueList.Add(VAR_SCALING_NORMAL_FAN_OUT)
			valueList.Add(VAR_SCALING_NORMAL_FAN_AVG)
			valueList.Add(VAR_SCALING_UNIFORM_FAN_IN)
			valueList.Add(VAR_SCALING_UNIFORM_FAN_OUT)
			valueList.Add(VAR_SCALING_UNIFORM_FAN_AVG)
		End Sub

		Public Enum InnerEnum
			DISTRIBUTION
			ZERO
			ONES
			SIGMOID_UNIFORM
			NORMAL
			LECUN_NORMAL
			UNIFORM
			XAVIER
			XAVIER_UNIFORM
			XAVIER_FAN_IN
			XAVIER_LEGACY
			RELU
			RELU_UNIFORM
			IDENTITY
			LECUN_UNIFORM
			VAR_SCALING_NORMAL_FAN_IN
			VAR_SCALING_NORMAL_FAN_OUT
			VAR_SCALING_NORMAL_FAN_AVG
			VAR_SCALING_UNIFORM_FAN_IN
			VAR_SCALING_UNIFORM_FAN_OUT
			VAR_SCALING_UNIFORM_FAN_AVG
		End Enum

		Public ReadOnly innerEnumValue As InnerEnum
		Private ReadOnly nameValue As String
		Private ReadOnly ordinalValue As Integer
		Private Shared nextOrdinal As Integer = 0

		Private Sub New(ByVal name As String, ByVal thisInnerEnumValue As InnerEnum)
			nameValue = name
			ordinalValue = nextOrdinal
			nextOrdinal += 1
			innerEnumValue = thisInnerEnumValue
		End Sub


		''' <summary>
		''' Create an instance of the weight initialization function
		''' </summary>
		''' <returns> a new <seealso cref="IWeightInit"/> instance </returns>
		Public ReadOnly Property WeightInitFunction As IWeightInit
			Get
				Return getWeightInitFunction(Nothing)
			End Get
		End Property

		''' <summary>
		''' Create an instance of the weight initialization function
		''' </summary>
		''' <param name="distribution"> Distribution of the weights (Only used in case DISTRIBUTION) </param>
		''' <returns> a new <seealso cref="IWeightInit"/> instance </returns>
		Public Function getWeightInitFunction(ByVal distribution As org.deeplearning4j.nn.conf.distribution.Distribution) As IWeightInit
			Select Case Me
				Case ZERO
					Return New WeightInitConstant(0.0)
				Case ONES
					Return New WeightInitConstant(1.0)
				Case WeightInit.DISTRIBUTION
					Return New WeightInitDistribution(distribution)
				Case SIGMOID_UNIFORM
					Return New WeightInitSigmoidUniform()
				Case LECUN_NORMAL, XAVIER_FAN_IN, NORMAL 'Fall through: these 3 are equivalent
					Return New WeightInitNormal()
				Case UNIFORM
					Return New WeightInitUniform()
				Case XAVIER
					Return New WeightInitXavier()
				Case XAVIER_UNIFORM
					Return New WeightInitXavierUniform()
				Case XAVIER_LEGACY
					Return New WeightInitXavierLegacy()
				Case RELU
					Return New WeightInitRelu()
				Case RELU_UNIFORM
					Return New WeightInitReluUniform()
				Case IDENTITY
					Return New WeightInitIdentity()
				Case LECUN_UNIFORM
					Return New WeightInitLecunUniform()
				Case VAR_SCALING_NORMAL_FAN_IN
					Return New WeightInitVarScalingNormalFanIn()
				Case VAR_SCALING_NORMAL_FAN_OUT
					Return New WeightInitVarScalingNormalFanOut()
				Case VAR_SCALING_NORMAL_FAN_AVG
					Return New WeightInitVarScalingNormalFanAvg()
				Case VAR_SCALING_UNIFORM_FAN_IN
					Return New WeightInitVarScalingUniformFanIn()
				Case VAR_SCALING_UNIFORM_FAN_OUT
					Return New WeightInitVarScalingUniformFanOut()
				Case VAR_SCALING_UNIFORM_FAN_AVG
					Return New WeightInitVarScalingUniformFanAvg()

				Case Else
					Throw New System.NotSupportedException("Unknown or not supported weight initialization function: " & Me)
			End Select
		End Function

		Public Shared Function values() As WeightInit()
			Return valueList.ToArray()
		End Function

		Public Function ordinal() As Integer
			Return ordinalValue
		End Function

		Public Overrides Function ToString() As String
			Return nameValue
		End Function

		Public Shared Operator =(ByVal one As WeightInit, ByVal two As WeightInit) As Boolean
			Return one.innerEnumValue = two.innerEnumValue
		End Operator

		Public Shared Operator <>(ByVal one As WeightInit, ByVal two As WeightInit) As Boolean
			Return one.innerEnumValue <> two.innerEnumValue
		End Operator

		Public Shared Function valueOf(ByVal name As String) As WeightInit
			For Each enumInstance As WeightInit In WeightInit.valueList
				If enumInstance.nameValue = name Then
					Return enumInstance
				End If
			Next
			Throw New System.ArgumentException(name)
		End Function
	End Class

End Namespace