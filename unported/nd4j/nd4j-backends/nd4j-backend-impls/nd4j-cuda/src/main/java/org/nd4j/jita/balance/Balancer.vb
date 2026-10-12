Imports System
Imports AllocationStatus = org.nd4j.jita.allocator.enums.AllocationStatus
Imports AllocationPoint = org.nd4j.jita.allocator.impl.AllocationPoint
Imports AllocationShape = org.nd4j.jita.allocator.impl.AllocationShape
Imports Configuration = org.nd4j.jita.conf.Configuration

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

Namespace org.nd4j.jita.balance

	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
	<Obsolete>
	Public Interface Balancer

		''' 
		''' <summary>
		''' This method initializes this Balancer instance
		''' </summary>
		''' <param name="configuration"> </param>
		Sub init(ByVal configuration As Configuration)

		''' <summary>
		''' This method checks, if it's worth moving some memory region to device
		''' </summary>
		''' <param name="deviceId"> </param>
		''' <param name="point"> </param>
		''' <param name="shape">
		''' @return </param>
		Function makePromoteDecision(ByVal deviceId As Integer?, ByVal point As AllocationPoint, ByVal shape As AllocationShape) As AllocationStatus

		''' <summary>
		''' This method checks, if it's worth moving some memory region to host
		''' </summary>
		''' <param name="deviceId"> </param>
		''' <param name="point"> </param>
		''' <param name="shape">
		''' @return </param>
		Function makeDemoteDecision(ByVal deviceId As Integer?, ByVal point As AllocationPoint, ByVal shape As AllocationShape) As AllocationStatus
	End Interface

End Namespace