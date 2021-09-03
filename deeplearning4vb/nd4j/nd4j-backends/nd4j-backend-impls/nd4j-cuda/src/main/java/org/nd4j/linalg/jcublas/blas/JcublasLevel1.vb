Imports val = lombok.val
Imports org.bytedeco.javacpp
Imports Preconditions = org.nd4j.common.base.Preconditions
Imports Allocator = org.nd4j.jita.allocator.Allocator
Imports AtomicAllocator = org.nd4j.jita.allocator.impl.AtomicAllocator
Imports cublasHandle_t = org.nd4j.jita.allocator.pointers.cuda.cublasHandle_t
Imports BaseLevel1 = org.nd4j.linalg.api.blas.impl.BaseLevel1
Imports DataBuffer = org.nd4j.linalg.api.buffer.DataBuffer
Imports DataType = org.nd4j.linalg.api.buffer.DataType
Imports INDArray = org.nd4j.linalg.api.ndarray.INDArray
Imports OpExecutionerUtil = org.nd4j.linalg.api.ops.executioner.OpExecutionerUtil
Imports ASum = org.nd4j.linalg.api.ops.impl.reduce.same.ASum
Imports Dot = org.nd4j.linalg.api.ops.impl.reduce3.Dot
Imports Axpy = org.nd4j.linalg.api.ops.impl.transforms.pairwise.arithmetic.Axpy
Imports DataTypeValidation = org.nd4j.linalg.factory.DataTypeValidation
Imports Nd4j = org.nd4j.linalg.factory.Nd4j
Imports CublasPointer = org.nd4j.linalg.jcublas.CublasPointer
Imports CudaContext = org.nd4j.linalg.jcublas.context.CudaContext
Imports CudaExecutioner = org.nd4j.linalg.jcublas.ops.executioner.CudaExecutioner
Imports NativeOps = org.nd4j.nativeblas.NativeOps
Imports NativeOpsHolder = org.nd4j.nativeblas.NativeOpsHolder
Imports Nd4jBlas = org.nd4j.nativeblas.Nd4jBlas
Imports Logger = org.slf4j.Logger
Imports LoggerFactory = org.slf4j.LoggerFactory
Imports org.bytedeco.cuda.cudart
Imports org.bytedeco.cuda.cublas
Imports org.bytedeco.cuda.global.cublas

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

Namespace org.nd4j.linalg.jcublas.blas

	''' <summary>
	''' @author Adam Gibson
	''' </summary>
	Public Class JcublasLevel1
		Inherits BaseLevel1

		Private allocator As Allocator = AtomicAllocator.Instance
		Private nd4jBlas As Nd4jBlas = DirectCast(Nd4j.factory().blas(), Nd4jBlas)
		Private nativeOps As NativeOps = NativeOpsHolder.Instance.getDeviceNativeOps()
		Private Shared logger As Logger = LoggerFactory.getLogger(GetType(JcublasLevel1))

		Protected Friend Overrides Function sdsdot(ByVal N As Long, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Single
			Throw New System.NotSupportedException()
		End Function

		Protected Friend Overrides Function dsdot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Double
			Throw New System.NotSupportedException()
		End Function


		Protected Friend Overrides Function hdot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Single
			DataTypeValidation.assertSameDataType(X, Y)
			'        CudaContext ctx = allocator.getFlowController().prepareAction(null, X, Y);

			Dim ret As Single = 1f

			'        CublasPointer xCPointer = new CublasPointer(X, ctx);
			'        CublasPointer yCPointer = new CublasPointer(Y, ctx);

			Dim dot As val = New Dot(X, Y)
			Nd4j.Executioner.exec(dot)

			ret = dot.getFinalResult().floatValue()
			Return ret
		End Function

		Protected Friend Overrides Function sdot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Single
			Preconditions.checkArgument(X.dataType() = DataType.FLOAT, "Float dtype expected")

			DataTypeValidation.assertSameDataType(X, Y)

			Nd4j.Executioner.push()

			Dim ctx As val = allocator.FlowController.prepareAction(Nothing, X, Y)

			Dim ret As Single = 1f

			Dim xCPointer As val = New CublasPointer(X, ctx)
			Dim yCPointer As val = New CublasPointer(Y, ctx)

			Dim handle As val = ctx.getCublasHandle()

			Dim cctx As val = New cublasContext(handle)
			SyncLock handle
				Dim result As Long = cublasSetStream_v2(cctx, New CUstream_st(ctx.getCublasStream()))
				If result <> 0 Then
					Throw New System.InvalidOperationException("cublasSetStream failed")
				End If

				Dim resultPointer As val = New FloatPointer(0.0f)
				result = cublasSdot_v2(cctx, CInt(N), CType(xCPointer.getDevicePointer(), FloatPointer), incX, CType(yCPointer.getDevicePointer(), FloatPointer), incY, resultPointer)

				If result <> 0 Then
					Throw New System.InvalidOperationException("cublasSdot_v2 failed. Error code: " & result)
				End If

				ret = resultPointer.get()
			End SyncLock

			allocator.registerAction(ctx, Nothing, X, Y)

			Return ret
		End Function

		Protected Friend Overrides Function hdot(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer, ByVal Y As DataBuffer, ByVal offsetY As Integer, ByVal incY As Integer) As Single
			Throw New System.NotSupportedException("not yet implemented")
		End Function

		Protected Friend Overrides Function sdot(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer, ByVal Y As DataBuffer, ByVal offsetY As Integer, ByVal incY As Integer) As Single
			Throw New System.NotSupportedException("not yet implemented")
		End Function

		Protected Friend Overrides Function ddot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer) As Double
			Preconditions.checkArgument(X.dataType() = DataType.DOUBLE, "Double dtype expected")

			Nd4j.Executioner.push()

			Dim ret As Double
			Dim ctx As val = allocator.FlowController.prepareAction(Nothing, X, Y)

			Dim xCPointer As val = New CublasPointer(X, ctx)
			Dim yCPointer As val = New CublasPointer(Y, ctx)

			Dim handle As val = ctx.getCublasHandle()
			SyncLock handle
				Dim cctx As val = New cublasContext(handle)
				cublasSetStream_v2(cctx, New CUstream_st(ctx.getCublasStream()))

				Dim resultPointer As val = New DoublePointer(0.0)
				cublasDdot_v2(cctx, CInt(N), CType(xCPointer.getDevicePointer(), DoublePointer), incX, CType(yCPointer.getDevicePointer(), DoublePointer), incY, resultPointer)
				ret = resultPointer.get()
			End SyncLock

			allocator.registerAction(ctx, Nothing, X, Y)

			Return ret
		End Function

		Protected Friend Overrides Function ddot(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer, ByVal Y As DataBuffer, ByVal offsetY As Integer, ByVal incY As Integer) As Double
			Throw New System.NotSupportedException("not yet implemented")
		End Function

		Protected Friend Overrides Function snrm2(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Single
			Preconditions.checkArgument(X.dataType() = DataType.FLOAT, "Float dtype expected")

			Nd4j.Executioner.push()


			Dim ctx As CudaContext = allocator.FlowController.prepareAction(Nothing, X)
			Dim ret As Single

			Dim cAPointer As New CublasPointer(X, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				Dim resultPointer As New FloatPointer(0.0f)
				cublasSnrm2_v2(New cublasContext(handle), CInt(N), CType(cAPointer.DevicePointer, FloatPointer), incX, resultPointer)
				ret = resultPointer.get()
			End SyncLock

			allocator.registerAction(ctx, Nothing, X)

			Return ret
		End Function

		Protected Friend Overrides Function hasum(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Single

			Dim asum As val = New ASum(X)
			Nd4j.Executioner.exec(asum)

			Dim ret As Single = asum.getFinalResult().floatValue()

			Return ret
		End Function

		Protected Friend Overrides Function sasum(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Single
			Dim asum As New ASum(X)
			Nd4j.Executioner.exec(asum)

			Dim ret As Single = asum.FinalResult.floatValue()

			Return ret
		End Function

		Protected Friend Overrides Function hasum(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Single
			Throw New System.NotSupportedException("not yet implemented")
		End Function

		Protected Friend Overrides Function sasum(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Single
			Throw New System.NotSupportedException("not yet implemented")
		End Function

		Protected Friend Overrides Function dnrm2(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Double
			Preconditions.checkArgument(X.dataType() = DataType.DOUBLE, "Double dtype expected")

			Nd4j.Executioner.push()

			Dim ret As Double

			Dim ctx As CudaContext = allocator.FlowController.prepareAction(Nothing, X)

			Dim cAPointer As New CublasPointer(X, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				Dim resultPointer As New DoublePointer(0.0f)
				cublasDnrm2_v2(New cublasContext(handle), CInt(N), CType(cAPointer.DevicePointer, DoublePointer), incX, resultPointer)
				ret = resultPointer.get()
			End SyncLock

			allocator.registerAction(ctx, Nothing, X)

			Return ret
		End Function

		Protected Friend Overrides Function dasum(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Double
			Dim asum As New ASum(X)
			Nd4j.Executioner.exec(asum)

			Dim ret As Double = asum.FinalResult.doubleValue()

			Return ret
		End Function

		Protected Friend Overrides Function dasum(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Double
			Throw New System.NotSupportedException("not yet implemented")
		End Function

		Protected Friend Overrides Function isamax(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Integer
			Preconditions.checkArgument(X.dataType() = DataType.FLOAT, "Float dtype expected")

			Nd4j.Executioner.push()

			Dim ctx As CudaContext = allocator.FlowController.prepareAction(Nothing, X)
			Dim ret2 As Integer

			Dim xCPointer As New CublasPointer(X, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				Dim resultPointer As New IntPointer(New Integer() {0})
				cublasIsamax_v2(New cublasContext(handle), CInt(N), CType(xCPointer.DevicePointer, FloatPointer), incX, resultPointer)
				ret2 = resultPointer.get()
			End SyncLock
			allocator.registerAction(ctx, Nothing, X)

			Return ret2 - 1
		End Function

		Protected Friend Overrides Function isamax(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Integer
			Throw New System.NotSupportedException("not yet implemented")
		End Function

		Protected Friend Overrides Function idamax(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer) As Integer
			Preconditions.checkArgument(X.dataType() = DataType.DOUBLE, "Double dtype expected")

			Nd4j.Executioner.push()

			Dim ctx As CudaContext = allocator.FlowController.prepareAction(Nothing, X)
			Dim ret2 As Integer

			Dim xCPointer As New CublasPointer(X, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				Dim resultPointer As New IntPointer(New Integer() {0})
				cublasIdamax_v2(New cublasContext(handle), CInt(N), CType(xCPointer.DevicePointer, DoublePointer), incX, resultPointer)
				ret2 = resultPointer.get()
			End SyncLock

			allocator.registerAction(ctx, Nothing, X)

			Return ret2 - 1
		End Function

		Protected Friend Overrides Function idamax(ByVal N As Long, ByVal X As DataBuffer, ByVal offsetX As Integer, ByVal incX As Integer) As Integer
			Throw New System.NotSupportedException("not yet implemented")
		End Function

		Protected Friend Overrides Sub sswap(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
			Preconditions.checkArgument(X.dataType() = DataType.FLOAT, "Float dtype expected")

			Nd4j.Executioner.push()

			Dim ctx As CudaContext = allocator.FlowController.prepareAction(Y, X)

			Dim xCPointer As New CublasPointer(X, ctx)
			Dim yCPointer As New CublasPointer(Y, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				cublasSswap_v2(New cublasContext(handle), CInt(N), CType(xCPointer.DevicePointer, FloatPointer), incX, CType(yCPointer.DevicePointer, FloatPointer), incY)
			End SyncLock

			allocator.registerAction(ctx, Y, X)
			OpExecutionerUtil.checkForAny(Y)
		End Sub

		Protected Friend Overrides Sub scopy(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
			Preconditions.checkArgument(X.dataType() = DataType.FLOAT, "Float dtype expected")

			Nd4j.Executioner.push()


			Dim ctx As CudaContext = allocator.FlowController.prepareAction(Y, X)

			Dim xCPointer As New CublasPointer(X, ctx)
			Dim yCPointer As New CublasPointer(Y, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				cublasScopy_v2(New cublasContext(handle), CInt(N), CType(xCPointer.DevicePointer, FloatPointer), incX, CType(yCPointer.DevicePointer, FloatPointer), incY)
			End SyncLock

			allocator.registerAction(ctx, Y, X)
			OpExecutionerUtil.checkForAny(Y)
		End Sub

		Protected Friend Overrides Sub scopy(ByVal N As Long, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)
			Throw New System.NotSupportedException("not yet implemented")
		End Sub

		Protected Friend Overrides Sub saxpy(ByVal N As Long, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
			'Preconditions.checkArgument(X.dataType() == DataType.FLOAT, "Float dtype expected");

			'        CudaContext ctx = allocator.getFlowController().prepareAction(Y, X);
			Nd4j.Executioner.exec(New Axpy(X, Y, Y, alpha))

			OpExecutionerUtil.checkForAny(Y)
		End Sub

		Protected Friend Overrides Sub haxpy(ByVal N As Long, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
			'        CudaContext ctx = allocator.getFlowController().prepareAction(Y, X);

			'        CublasPointer xAPointer = new CublasPointer(X, ctx);
			'        CublasPointer xBPointer = new CublasPointer(Y, ctx);

			'        cublasHandle_t handle = ctx.getCublasHandle();

			DirectCast(Nd4j.Executioner, CudaExecutioner).exec(New Axpy(X, Y, Y, alpha))

			OpExecutionerUtil.checkForAny(Y)
		End Sub

		Protected Friend Overrides Sub haxpy(ByVal N As Long, ByVal alpha As Single, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)
			Throw New System.NotSupportedException("not yet implemented")
		End Sub

		Protected Friend Overrides Sub saxpy(ByVal N As Long, ByVal alpha As Single, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)
			Throw New System.NotSupportedException("not yet implemented")
		End Sub

		Protected Friend Overrides Sub dswap(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)

			Nd4j.Executioner.push()

			Dim ctx As CudaContext = allocator.FlowController.prepareAction(Y, X)

			Dim xCPointer As New CublasPointer(X, ctx)
			Dim yCPointer As New CublasPointer(Y, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				cublasDswap_v2(New cublasContext(handle), CInt(N), CType(xCPointer.DevicePointer, DoublePointer), incX, CType(yCPointer.DevicePointer, DoublePointer), incY)
			End SyncLock

			allocator.registerAction(ctx, Y, X)

			OpExecutionerUtil.checkForAny(Y)
		End Sub

		Protected Friend Overrides Sub dcopy(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
			Nd4j.Executioner.push()

			Dim ctx As CudaContext = allocator.FlowController.prepareAction(Y, X)

			Dim xCPointer As New CublasPointer(X, ctx)
			Dim yCPointer As New CublasPointer(Y, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				cublasDcopy_v2(New cublasContext(handle), CInt(N), CType(xCPointer.DevicePointer, DoublePointer), incX, CType(yCPointer.DevicePointer, DoublePointer), incY)
			End SyncLock

			allocator.registerAction(ctx, Y, X)

			OpExecutionerUtil.checkForAny(Y)
		End Sub

		Protected Friend Overrides Sub dcopy(ByVal N As Long, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)
			Throw New System.NotSupportedException("not yet implemented")
		End Sub

		Protected Friend Overrides Sub daxpy(ByVal N As Long, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer)
			'CudaContext ctx = allocator.getFlowController().prepareAction(Y, X);


			'    logger.info("incX: {}, incY: {}, N: {}, X.length: {}, Y.length: {}", incX, incY, N, X.length(), Y.length());

			Nd4j.Executioner.exec(New Axpy(X, Y, Y, alpha))

			OpExecutionerUtil.checkForAny(Y)
		End Sub

		Protected Friend Overrides Sub daxpy(ByVal N As Long, ByVal alpha As Double, ByVal x As DataBuffer, ByVal offsetX As Integer, ByVal incrX As Integer, ByVal y As DataBuffer, ByVal offsetY As Integer, ByVal incrY As Integer)
			Throw New System.NotSupportedException("not yet implemented")
		End Sub

		Protected Friend Overrides Sub srotg(ByVal a As Single, ByVal b As Single, ByVal c As Single, ByVal s As Single)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub srotmg(ByVal d1 As Single, ByVal d2 As Single, ByVal b1 As Single, ByVal b2 As Single, ByVal P As INDArray)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub srot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal c As Single, ByVal s As Single)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub srotm(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal P As INDArray)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub drotg(ByVal a As Double, ByVal b As Double, ByVal c As Double, ByVal s As Double)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub drotmg(ByVal d1 As Double, ByVal d2 As Double, ByVal b1 As Double, ByVal b2 As Double, ByVal P As INDArray)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub drot(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal c As Double, ByVal s As Double)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub drotm(ByVal N As Long, ByVal X As INDArray, ByVal incX As Integer, ByVal Y As INDArray, ByVal incY As Integer, ByVal P As INDArray)
			Throw New System.NotSupportedException()

		End Sub

		Protected Friend Overrides Sub sscal(ByVal N As Long, ByVal alpha As Single, ByVal X As INDArray, ByVal incX As Integer)
			Preconditions.checkArgument(X.dataType() = DataType.FLOAT, "Float dtype expected")

			Nd4j.Executioner.push()

			Dim ctx As CudaContext = allocator.FlowController.prepareAction(X)

			Dim xCPointer As New CublasPointer(X, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				cublasSscal_v2(New cublasContext(handle),CInt(N), New FloatPointer(alpha), CType(xCPointer.DevicePointer, FloatPointer), incX)
			End SyncLock

			allocator.registerAction(ctx, X)

			OpExecutionerUtil.checkForAny(X)
		End Sub

		Protected Friend Overrides Sub dscal(ByVal N As Long, ByVal alpha As Double, ByVal X As INDArray, ByVal incX As Integer)
			Preconditions.checkArgument(X.dataType() = DataType.DOUBLE, "Double dtype expected")

			Nd4j.Executioner.push()

			Dim ctx As CudaContext = allocator.FlowController.prepareAction(X)

			Dim xCPointer As New CublasPointer(X, ctx)

			Dim handle As cublasHandle_t = ctx.CublasHandle
			SyncLock handle
				cublasSetStream_v2(New cublasContext(handle), New CUstream_st(ctx.CublasStream))

				cublasDscal_v2(New cublasContext(handle), CInt(N), New DoublePointer(alpha), CType(xCPointer.DevicePointer, DoublePointer), incX)
			End SyncLock

			allocator.registerAction(ctx, X)

			OpExecutionerUtil.checkForAny(X)
		End Sub

		Public Overrides Function supportsDataBufferL1Ops() As Boolean
			Return False
		End Function
	End Class

End Namespace