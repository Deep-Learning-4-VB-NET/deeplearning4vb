Imports Getter = lombok.Getter
Imports NonNull = lombok.NonNull
Imports val = lombok.val
Imports Allocator = org.nd4j.jita.allocator.Allocator
Imports AllocationStatus = org.nd4j.jita.allocator.enums.AllocationStatus
Imports AllocationPoint = org.nd4j.jita.allocator.impl.AllocationPoint
Imports cudaStream_t = org.nd4j.jita.allocator.pointers.cuda.cudaStream_t
Imports EventsProvider = org.nd4j.jita.concurrency.EventsProvider
Imports Configuration = org.nd4j.jita.conf.Configuration
Imports CudaEnvironment = org.nd4j.jita.conf.CudaEnvironment
Imports FlowController = org.nd4j.jita.flow.FlowController
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports AffinityManager = org.nd4j.linalg.api.concurrency.AffinityManager
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports JCublasNDArray = org.nd4j.linalg.jcublas.JCublasNDArray
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext
Imports NativeOps = org.nd4j.nativeblas.NativeOps
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory

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

Namespace org.nd4j.jita.flow.impl


	''' <summary>
	''' @author raver119@gmail.com
	''' </summary>
	Public Class SynchronousFlowController
		Implements FlowController

		Private Shared log As Logger = LoggerFactory.getLogger(GetType(SynchronousFlowController))
'JAVA TO VB CONVERTER TODO TASK: There is no VB equivalent to 'volatile':
'ORIGINAL LINE: private volatile org.nd4j.jita.allocator.Allocator allocator;
		Private allocator As Allocator
		Protected Friend nativeOps As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()
		Protected Friend configuration As Configuration = CudaEnvironment.Instance.Configuration
'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Getter protected org.nd4j.jita.concurrency.EventsProvider eventsProvider = new org.nd4j.jita.concurrency.EventsProvider();
		Protected Friend eventsProvider As New EventsProvider()

		Public Overridable Sub init(ByVal allocator As Allocator) Implements FlowController.init
			Me.allocator = allocator
		End Sub

		''' <summary>
		''' This method makes sure HOST memory contains latest data from GPU
		''' </summary>
		''' <param name="point"> </param>
		Public Overridable Sub synchronizeToHost(ByVal point As AllocationPoint) Implements FlowController.synchronizeToHost
			NativeOpsHolder.Instance.getDeviceNativeOps().dbSyncToPrimary(point.getPtrDataBuffer())
		End Sub

'JAVA TO VB CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
'ORIGINAL LINE: @Override public void synchronizeToDevice(@NonNull AllocationPoint point)
		Public Overridable Sub synchronizeToDevice(ByVal point As AllocationPoint)
			NativeOpsHolder.Instance.getDeviceNativeOps().dbSyncToSpecial(point.getPtrDataBuffer())
		End Sub

		Public Overridable Sub waitTillFinished(ByVal point As AllocationPoint) Implements FlowController.waitTillFinished
			' this should be always null, since synchronization happens in C++ now
			If point.getLastWriteEvent() IsNot Nothing Then
				point.getLastWriteEvent().synchronize()
			End If
		End Sub


		Public Overridable Function prepareActionAllWrite(ParamArray ByVal operands() As INDArray) As CudaContext Implements FlowController.prepareActionAllWrite
			Dim context As val = allocator.DeviceContext
			Dim cId As val = allocator.getDeviceId()

			For Each operand As INDArray In operands
				If operand Is Nothing OrElse operand.Empty Then
					Continue For
				End If

				Nd4j.Compressor.autoDecompress(operand)

				Dim pointData As val = allocator.getAllocationPoint(operand)
				Dim pointShape As val = allocator.getAllocationPoint(operand.shapeInfoDataBuffer())


				If pointData.getDeviceId() <> cId AndAlso pointData.getDeviceId() >= 0 Then
					Dim buffer As DataBuffer = If(operand.data().originalDataBuffer() Is Nothing, operand.data(), operand.data().originalDataBuffer())
					allocator.MemoryHandler.relocateObject(buffer)
				End If

				If pointShape.getDeviceId() <> cId AndAlso pointShape.getDeviceId() >= 0 Then
					DirectCast(operand, JCublasNDArray).ShapeInfoDataBuffer = Nd4j.ConstantHandler.relocateConstantSpace(operand.shapeInfoDataBuffer())
				End If

				prepareDelayedMemory(operand)
				allocator.getAllocationPoint(operand).CurrentContext = context
			Next operand
			Return context
		End Function

		Public Overridable Function prepareAction(ByVal result As INDArray, ParamArray ByVal operands() As INDArray) As CudaContext Implements FlowController.prepareAction
			Dim context As val = allocator.DeviceContext
			Dim cId As val = allocator.getDeviceId()


			If result IsNot Nothing AndAlso Not result.Empty Then
				Nd4j.Compressor.autoDecompress(result)
				prepareDelayedMemory(result)
				Dim pointData As val = allocator.getAllocationPoint(result)
				Dim pointShape As val = allocator.getAllocationPoint(result.shapeInfoDataBuffer())

				If pointData.getDeviceId() <> cId AndAlso pointData.getDeviceId() >= 0 AndAlso (Not CudaEnvironment.Instance.Configuration.isCrossDeviceAccessAllowed() OrElse Not NativeOpsHolder.Instance.getDeviceNativeOps().isP2PAvailable()) Then
					Dim buffer As DataBuffer = If(result.data().originalDataBuffer() Is Nothing, result.data(), result.data().originalDataBuffer())
					allocator.MemoryHandler.relocateObject(buffer)
				End If

				If pointShape.getDeviceId() <> cId AndAlso pointShape.getDeviceId() >= 0 Then
					DirectCast(result, JCublasNDArray).ShapeInfoDataBuffer = Nd4j.Executioner.createShapeInfo(result.shape(), result.stride(), result.elementWiseStride(), result.ordering(), result.dataType(), result.Empty)
				End If

				allocator.getAllocationPoint(result).CurrentContext = context
			End If

			If operands Is Nothing Then
				Return context
			End If

			For Each operand As INDArray In operands
				' empty or String arrays can be skipped
				If operand Is Nothing OrElse operand.Empty OrElse operand.S Then
					Continue For
				End If

				Nd4j.Compressor.autoDecompress(operand)

				Dim pointData As val = allocator.getAllocationPoint(operand)
				Dim pointShape As val = allocator.getAllocationPoint(operand.shapeInfoDataBuffer())
				Nd4j.AffinityManager.ensureLocation(operand, AffinityManager.Location.DEVICE)

				If pointData.getDeviceId() <> cId AndAlso pointData.getDeviceId() >= 0 AndAlso (Not CudaEnvironment.Instance.Configuration.isCrossDeviceAccessAllowed() OrElse Not NativeOpsHolder.Instance.getDeviceNativeOps().isP2PAvailable()) Then
					Dim buffer As DataBuffer = If(operand.data().originalDataBuffer() Is Nothing, operand.data(), operand.data().originalDataBuffer())
					allocator.MemoryHandler.relocateObject(buffer)
				End If

				If pointShape.getDeviceId() <> cId AndAlso pointShape.getDeviceId() >= 0 Then
					DirectCast(operand, JCublasNDArray).ShapeInfoDataBuffer = Nd4j.Executioner.createShapeInfo(operand.shape(), operand.stride(), operand.elementWiseStride(), operand.ordering(), operand.dataType(), operand.Empty)
				End If

				prepareDelayedMemory(operand)
				allocator.getAllocationPoint(operand).CurrentContext = context
			Next operand
			Return context
		End Function

		Public Overridable Sub waitTillReleased(ByVal point As AllocationPoint) Implements FlowController.waitTillReleased
			waitTillFinished(point)

			If point.getLastReadEvent() IsNot Nothing Then
				point.getLastReadEvent().synchronize()
			End If
		End Sub

		Public Overridable Sub registerAction(ByVal context As CudaContext, ByVal result As AllocationPoint, ParamArray ByVal operands() As AllocationPoint) Implements FlowController.registerAction
			' this method is irrelevant now, everything happens in C++ now
	'        
	'        eventsProvider.storeEvent(result.getLastWriteEvent());
	'        result.setLastWriteEvent(eventsProvider.getEvent());
	'        result.getLastWriteEvent().register(context.getOldStream());
	'
	'
	'        for (AllocationPoint operand : operands) {
	'            eventsProvider.storeEvent(operand.getLastReadEvent());
	'            operand.setLastReadEvent(eventsProvider.getEvent());
	'            operand.getLastReadEvent().register(context.getOldStream());
	'        }
	'        //   context.syncOldStream();
	'        
		End Sub

		Public Overridable Sub registerActionAllWrite(ByVal context As CudaContext, ParamArray ByVal operands() As INDArray) Implements FlowController.registerActionAllWrite
			For Each operand As INDArray In operands
				If operand Is Nothing Then
					Continue For
				End If

				Dim pointOperand As val = allocator.getAllocationPoint(operand)
				pointOperand.tickDeviceWrite()
			Next operand
		End Sub

		Public Overridable Sub registerAction(ByVal context As CudaContext, ByVal result As INDArray, ParamArray ByVal operands() As INDArray) Implements FlowController.registerAction
			If result Is Nothing OrElse result.Empty Then
				Return
			End If

			Dim point As val = allocator.getAllocationPoint(result)
			point.tickDeviceWrite()

			For Each operand As INDArray In operands
				If operand Is Nothing OrElse operand.Empty Then
					Continue For
				End If

				Dim pointOperand As val = allocator.getAllocationPoint(operand)
				pointOperand.tickDeviceRead()
			Next operand
		End Sub

		Public Overridable Function prepareAction(ByVal result As AllocationPoint, ParamArray ByVal operands() As AllocationPoint) As CudaContext Implements FlowController.prepareAction
			Dim context As val = allocator.DeviceContext

			If result IsNot Nothing Then
				result.CurrentContext = context
			End If

			For Each operand As AllocationPoint In operands
				If operand Is Nothing Then
					Continue For
				End If

				operand.CurrentContext = context
			Next operand

			Return context
		End Function

		Public Overridable Sub commitTransfer(ByVal streamUsed As cudaStream_t) Implements FlowController.commitTransfer
			streamUsed.synchronize()
		End Sub

		Protected Friend Overridable Sub prepareDelayedMemory(ByVal array As INDArray)
			If configuration.getMemoryModel() = Configuration.MemoryModel.DELAYED Then
				Dim pointData As val = allocator.getAllocationPoint(array.shapeInfoDataBuffer())
				Dim pointShape As val = allocator.getAllocationPoint(array.shapeInfoDataBuffer())

				If pointData.getAllocationStatus() <> AllocationStatus.DEVICE Then
					prepareDelayedMemory(array.data())
				End If

				If pointShape.getAllocationStatus() = AllocationStatus.HOST Then
					Dim oShape As val = array.shapeInfoDataBuffer()
					Dim nShape As val = Nd4j.ConstantHandler.relocateConstantSpace(oShape)

					If nShape = oShape Then
						Nd4j.ConstantHandler.moveToConstantSpace(nShape)
					End If
					DirectCast(array, JCublasNDArray).ShapeInfoDataBuffer = nShape
				End If
			End If
		End Sub

		Protected Friend Overridable Sub prepareDelayedMemory(ByVal buffer As DataBuffer)

			allocator.MemoryHandler.promoteObject(buffer)
		End Sub
	End Class

End Namespace