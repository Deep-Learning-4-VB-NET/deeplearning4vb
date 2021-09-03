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

Namespace org.nd4j.jita.allocator.time

	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
	Public Interface RateTimer

		''' <summary>
		''' This method notifies timer about event
		''' </summary>
		Sub triggerEvent()

		''' <summary>
		''' This method returns average frequency of events happened within predefined timeframe
		''' @return
		''' </summary>
		ReadOnly Property FrequencyOfEvents As Double

		''' <summary>
		''' This method returns total number of events happened withing predefined timeframe
		''' @return
		''' </summary>
		ReadOnly Property NumberOfEvents As Long
	End Interface

End Namespace